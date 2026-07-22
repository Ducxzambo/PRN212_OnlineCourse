using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;

namespace Presentation.ViewModels;

public class StudentCourseListViewModel : ViewModelBase
{
    private List<Enrollment> _courses = new();
    private Enrollment? _selectedCourse;
    private string? _errorMessage;
    private bool _isLoading;

    public List<Enrollment> Courses
    {
        get => _courses;
        set => SetProperty(ref _courses, value);
    }

    public Enrollment? SelectedCourse
    {
        get => _selectedCourse;
        set => SetProperty(ref _selectedCourse, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand ViewCourseCommand { get; }
    public event EventHandler? ViewCourseRequested;

    public StudentCourseListViewModel()
    {
        ViewCourseCommand = new RelayCommand(ViewCourse, () => SelectedCourse != null);
    }

    public async Task LoadAsync()
    {
        if (StudentSession.Current == null) return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var enrollments = await AppServices.StudentService.GetEnrollmentHistoryAsync(StudentSession.Current.Id);
            Courses = enrollments;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tải danh sách khóa học: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ViewCourse()
    {
        if (SelectedCourse != null)
        {
            ViewCourseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
