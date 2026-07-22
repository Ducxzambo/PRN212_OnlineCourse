using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;
using Presentation.Views;

namespace Presentation.ViewModels;

public class StudentListViewModel : ViewModelBase
{
    private List<Enrollment> _allEnrollments = new();
    private string _searchText = "";
    private CourseFilterOption? _selectedCourseFilter;
    private Enrollment? _selectedEnrollment;
    private int _selectedStatusValue;

    private string _newStudentFullName = "";
    private string _newStudentEmail = "";
    private string? _newStudentPhone;
    private Course? _selectedEnrollCourse;

    public ObservableCollection<Enrollment> Enrollments { get; } = new();
    public ObservableCollection<CourseFilterOption> CourseFilters { get; } = new();
    public ObservableCollection<Course> MyCourses { get; } = new();
    public List<EnrollmentStatusOption> StatusOptions => EnrollmentStatusOptions.All;

    public string SearchText
    {
        get => _searchText;
        set { if (SetProperty(ref _searchText, value)) ApplyFilter(); }
    }

    public CourseFilterOption? SelectedCourseFilter
    {
        get => _selectedCourseFilter;
        set { if (SetProperty(ref _selectedCourseFilter, value)) ApplyFilter(); }
    }

    public Enrollment? SelectedEnrollment
    {
        get => _selectedEnrollment;
        set
        {
            if (SetProperty(ref _selectedEnrollment, value))
                SelectedStatusValue = value?.Status ?? 0;
        }
    }

    public int SelectedStatusValue
    {
        get => _selectedStatusValue;
        set => SetProperty(ref _selectedStatusValue, value);
    }

    public string NewStudentFullName
    {
        get => _newStudentFullName;
        set => SetProperty(ref _newStudentFullName, value);
    }

    public string NewStudentEmail
    {
        get => _newStudentEmail;
        set => SetProperty(ref _newStudentEmail, value);
    }

    public string? NewStudentPhone
    {
        get => _newStudentPhone;
        set => SetProperty(ref _newStudentPhone, value);
    }

    public Course? SelectedEnrollCourse
    {
        get => _selectedEnrollCourse;
        set => SetProperty(ref _selectedEnrollCourse, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand UpdateStatusCommand { get; }
    public ICommand RemoveEnrollmentCommand { get; }
    public ICommand EnrollCommand { get; }
    public ICommand ViewHistoryCommand { get; }

    public StudentListViewModel()
    {
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        UpdateStatusCommand = new AsyncRelayCommand(UpdateStatusAsync, () => SelectedEnrollment != null);
        RemoveEnrollmentCommand = new AsyncRelayCommand(RemoveEnrollmentAsync, () => SelectedEnrollment != null);
        EnrollCommand = new AsyncRelayCommand(EnrollAsync);
        ViewHistoryCommand = new RelayCommand(ViewHistory, () => SelectedEnrollment != null);
    }

    public async Task LoadAsync()
    {
        if (InstructorSession.Current == null) return;
        var instructorId = InstructorSession.Current.Id;

        var previousSelectedEnrollmentId = SelectedEnrollment?.Id;
        var previousFilterCourseId = SelectedCourseFilter?.CourseId;
        var previousEnrollCourseId = SelectedEnrollCourse?.Id;

        _allEnrollments = await AppServices.StudentService.GetRosterByInstructorAsync(instructorId);
        var courses = await AppServices.CourseService.GetCoursesByInstructorAsync(instructorId);

        MyCourses.Clear();
        foreach (var course in courses) MyCourses.Add(course);
        SelectedEnrollCourse = MyCourses.FirstOrDefault(c => c.Id == previousEnrollCourseId) ?? MyCourses.FirstOrDefault();

        CourseFilters.Clear();
        CourseFilters.Add(new CourseFilterOption { CourseId = null, DisplayName = "-- Tất cả khóa học --" });
        foreach (var course in courses)
            CourseFilters.Add(new CourseFilterOption { CourseId = course.Id, DisplayName = course.Title });
        SelectedCourseFilter = CourseFilters.FirstOrDefault(f => f.CourseId == previousFilterCourseId) ?? CourseFilters.First();

        ApplyFilter();

        if (previousSelectedEnrollmentId.HasValue)
            SelectedEnrollment = Enrollments.FirstOrDefault(e => e.Id == previousSelectedEnrollmentId.Value);
    }

    private void ApplyFilter()
    {
        IEnumerable<Enrollment> filtered = _allEnrollments;

        if (SelectedCourseFilter?.CourseId != null)
            filtered = filtered.Where(e => e.CourseId == SelectedCourseFilter.CourseId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            filtered = filtered.Where(e =>
                e.Student.Account.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                e.Student.Account.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        Enrollments.Clear();
        foreach (var enrollment in filtered) Enrollments.Add(enrollment);
    }

    private async Task UpdateStatusAsync()
    {
        if (SelectedEnrollment == null) return;

        var (success, error) = await AppServices.StudentService.UpdateEnrollmentStatusAsync(SelectedEnrollment.Id, SelectedStatusValue);
        if (!success)
        {
            MessageBox.Show(error, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        await LoadAsync();
    }

    private async Task RemoveEnrollmentAsync()
    {
        if (SelectedEnrollment == null) return;

        var confirm = MessageBox.Show(
            $"Xóa đăng ký của '{SelectedEnrollment.Student.Account.FullName}' khỏi khóa học '{SelectedEnrollment.Course.Title}'?",
            "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        await AppServices.StudentService.RemoveEnrollmentAsync(SelectedEnrollment.Id);
        await LoadAsync();
    }

    private async Task EnrollAsync()
    {
        if (SelectedEnrollCourse == null)
        {
            MessageBox.Show("Vui lòng chọn khóa học để ghi danh.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var (success, error) = await AppServices.StudentService.EnrollStudentAsync(
            SelectedEnrollCourse.Id, NewStudentFullName, NewStudentEmail, NewStudentPhone);

        if (!success)
        {
            MessageBox.Show(error, "Không thể ghi danh", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        NewStudentFullName = "";
        NewStudentEmail = "";
        NewStudentPhone = null;

        await LoadAsync();
    }

    private void ViewHistory()
    {
        if (SelectedEnrollment == null) return;

        var window = new StudentDetailWindow(new StudentDetailViewModel(SelectedEnrollment.Student));
        window.ShowDialog();
    }
}
