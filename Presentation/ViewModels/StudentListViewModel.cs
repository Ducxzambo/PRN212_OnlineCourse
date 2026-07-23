using System.Collections.ObjectModel;
using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;
using Presentation.Views;

namespace Presentation.ViewModels;

/// <summary>Read-only roster for instructors, with access to individual lesson progress.</summary>
public class StudentListViewModel : ViewModelBase
{
    private List<Enrollment> _allEnrollments = new();
    private string _searchText = "";
    private CourseFilterOption? _selectedCourseFilter;
    private Enrollment? _selectedEnrollment;

    public ObservableCollection<Enrollment> Enrollments { get; } = new();
    public ObservableCollection<CourseFilterOption> CourseFilters { get; } = new();

    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public CourseFilterOption? SelectedCourseFilter { get => _selectedCourseFilter; set { if (SetProperty(ref _selectedCourseFilter, value)) ApplyFilter(); } }
    public Enrollment? SelectedEnrollment { get => _selectedEnrollment; set => SetProperty(ref _selectedEnrollment, value); }

    public ICommand RefreshCommand { get; }
    public ICommand ViewProgressCommand { get; }

    public StudentListViewModel()
    {
        RefreshCommand = new RelayCommand(Load);
        ViewProgressCommand = new RelayCommand(ViewProgress, () => SelectedEnrollment != null);
    }

    public void Load()
    {
        if (InstructorSession.Current == null) return;
        var selectedEnrollmentId = SelectedEnrollment?.Id;
        var selectedFilterCourseId = SelectedCourseFilter?.CourseId;
        _allEnrollments = AppServices.StudentService.GetRosterByInstructor(InstructorSession.Current.Id);
        var courses = AppServices.CourseService.GetCoursesByInstructor(InstructorSession.Current.Id);

        CourseFilters.Clear();
        CourseFilters.Add(new CourseFilterOption { CourseId = null, DisplayName = "-- Tất cả khóa học --" });
        foreach (var course in courses) CourseFilters.Add(new CourseFilterOption { CourseId = course.Id, DisplayName = course.Title });
        SelectedCourseFilter = CourseFilters.FirstOrDefault(f => f.CourseId == selectedFilterCourseId) ?? CourseFilters.First();

        ApplyFilter();
        SelectedEnrollment = Enrollments.FirstOrDefault(e => e.Id == selectedEnrollmentId);
    }

    private void ApplyFilter()
    {
        IEnumerable<Enrollment> filtered = _allEnrollments;
        if (SelectedCourseFilter?.CourseId != null) filtered = filtered.Where(e => e.CourseId == SelectedCourseFilter.CourseId);
        if (!string.IsNullOrWhiteSpace(SearchText))
            filtered = filtered.Where(e => e.Student.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || e.Student.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        Enrollments.Clear();
        foreach (var enrollment in filtered) Enrollments.Add(enrollment);
    }

    private void ViewProgress()
    {
        if (SelectedEnrollment == null) return;
        new EnrollmentProgressWindow(new EnrollmentProgressViewModel(SelectedEnrollment)).ShowDialog();
    }
}
