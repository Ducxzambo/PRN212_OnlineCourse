using System.Windows.Input;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Presentation.Helpers;

namespace Presentation.ViewModels;

public class StudentLessonListViewModel : ViewModelBase
{
    private Course? _currentCourse;
    private List<Lesson> _lessons = new();
    private Lesson? _selectedLesson;
    private string? _errorMessage;
    private bool _isLoading;

    public Course? CurrentCourse
    {
        get => _currentCourse;
        set => SetProperty(ref _currentCourse, value);
    }

    public List<Lesson> Lessons
    {
        get => _lessons;
        set => SetProperty(ref _lessons, value);
    }

    public Lesson? SelectedLesson
    {
        get => _selectedLesson;
        set => SetProperty(ref _selectedLesson, value);
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

    public ICommand ViewLessonCommand { get; }
    public event EventHandler? ViewLessonRequested;

    public StudentLessonListViewModel()
    {
        ViewLessonCommand = new RelayCommand(ViewLesson, () => SelectedLesson != null);
    }

    public async Task LoadAsync(int courseId)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var lessons = await AppServices.StudentService.GetLessonsByCourseAsync(courseId);
            Lessons = lessons;

            // Optionally load course details from repository
            using var context = new OnlineCourseManagementDbContext();
            CurrentCourse = await context.Courses
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tải danh sách bài học: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ViewLesson()
    {
        if (SelectedLesson != null)
        {
            ViewLessonRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
