using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;

namespace Presentation.ViewModels;

public class StudentMainViewModel : ViewModelBase
{
    private Course? _selectedCourse;
    private Enrollment? _selectedEnrollment;
    private LessonProgressItem? _selectedLesson;

    public ObservableCollection<Course> Courses { get; } = new();
    public ObservableCollection<Enrollment> MyEnrollments { get; } = new();
    public ObservableCollection<LessonProgressItem> Lessons { get; } = new();

    public Course? SelectedCourse { get => _selectedCourse; set => SetProperty(ref _selectedCourse, value); }
    public Enrollment? SelectedEnrollment
    {
        get => _selectedEnrollment;
        set { if (SetProperty(ref _selectedEnrollment, value)) LoadLessons(); }
    }
    public LessonProgressItem? SelectedLesson { get => _selectedLesson; set => SetProperty(ref _selectedLesson, value); }
    public string StudentName => StudentSession.Current?.Account?.FullName ?? "Student";

    public ICommand RefreshCommand { get; }
    public ICommand EnrollCommand { get; }
    public ICommand CompleteLessonCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand OpenProfileCommand { get; }
    public event EventHandler? LogoutRequested;
    public event EventHandler? ProfileRequested;

    public StudentMainViewModel()
    {
        RefreshCommand = new RelayCommand(Load);
        EnrollCommand = new RelayCommand(Enroll, () => SelectedCourse != null);
        CompleteLessonCommand = new RelayCommand(UpdateLesson, () => SelectedEnrollment != null && SelectedLesson != null);
        LogoutCommand = new RelayCommand(Logout);
        OpenProfileCommand = new RelayCommand(() => ProfileRequested?.Invoke(this, EventArgs.Empty));
    }

    public void Load()
    {
        if (StudentSession.Current == null) return;
        var selectedEnrollmentId = SelectedEnrollment?.Id;
        var courses = AppServices.CourseService.GetAllCourses();
        var enrollments = AppServices.StudentService.GetEnrollmentHistory(StudentSession.Current.Id);

        Courses.Clear();
        foreach (var course in courses) Courses.Add(course);

        MyEnrollments.Clear();
        foreach (var enrollment in enrollments) MyEnrollments.Add(enrollment);
        SelectedEnrollment = MyEnrollments.FirstOrDefault(e => e.Id == selectedEnrollmentId) ?? MyEnrollments.FirstOrDefault();
    }

    private void Enroll()
    {
        if (StudentSession.Current == null || SelectedCourse == null) return;
        var result = AppServices.StudentService.EnrollExistingStudent(StudentSession.Current.Id, SelectedCourse.Id);
        if (!result.Success) MessageBox.Show(result.Error, "Thông báo");
        Load();
    }

    private void LoadLessons()
    {
        Lessons.Clear();
        if (SelectedEnrollment == null) return;

        var completedLessonIds = AppServices.StudentService.GetCompletedLessonIds(SelectedEnrollment.Id).ToHashSet();
        foreach (var lesson in AppServices.LessonService.GetLessonsByCourse(SelectedEnrollment.CourseId))
            Lessons.Add(new LessonProgressItem { Lesson = lesson, IsCompleted = completedLessonIds.Contains(lesson.Id) });
    }

    private void UpdateLesson()
    {
        if (SelectedEnrollment == null || SelectedLesson == null) return;
        var result = AppServices.StudentService.SetLessonCompletion(
            SelectedEnrollment.Id, SelectedLesson.Lesson.Id, !SelectedLesson.IsCompleted);
        if (!result.Success)
        {
            MessageBox.Show(result.Error, "Thông báo");
            return;
        }

        Load();
        LoadLessons();
    }

    private void Logout()
    {
        StudentSession.Current = null;
        LogoutRequested?.Invoke(this, EventArgs.Empty);
    }
}
