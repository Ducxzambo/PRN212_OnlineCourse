using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;

namespace Presentation.ViewModels;

public class StudentMainViewModel : ViewModelBase
{
    private object? _currentViewModel;
    private string _studentName = "";
    private int _enrollmentCount;
    private List<Enrollment> _enrolledCourses = new();

    private readonly StudentCourseListViewModel _courseListViewModel;
    private readonly StudentProgressViewModel _progressViewModel;

    public object? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public string StudentName
    {
        get => _studentName;
        set => SetProperty(ref _studentName, value);
    }

    public int EnrollmentCount
    {
        get => _enrollmentCount;
        set => SetProperty(ref _enrollmentCount, value);
    }

    public List<Enrollment> EnrolledCourses
    {
        get => _enrolledCourses;
        set => SetProperty(ref _enrolledCourses, value);
    }

    public ICommand NavigateCoursesCommand { get; }
    public ICommand NavigateProgressCommand { get; }
    public ICommand LogoutCommand { get; }

    /// <summary>Raised when the student logs out - the shell view closes and shows the login window again.</summary>
    public event EventHandler? LogoutRequested;

    public StudentMainViewModel()
    {
        _courseListViewModel = new StudentCourseListViewModel();
        _progressViewModel = new StudentProgressViewModel();

        NavigateCoursesCommand = new RelayCommand(() => CurrentViewModel = _courseListViewModel);
        NavigateProgressCommand = new RelayCommand(() => CurrentViewModel = _progressViewModel);
        LogoutCommand = new RelayCommand(Logout);

        CurrentViewModel = _courseListViewModel;
    }

    public async Task LoadAsync()
    {
        if (StudentSession.Current == null) return;

        StudentName = StudentSession.Current.Account?.FullName ?? "Student";

        await _courseListViewModel.LoadAsync();
        await _progressViewModel.LoadAsync();

        // Load enrollment summary
        var enrollments = await AppServices.StudentService.GetEnrollmentHistoryAsync(StudentSession.Current.Id);
        EnrolledCourses = enrollments;
        EnrollmentCount = enrollments.Count;
    }

    private void Logout()
    {
        StudentSession.Current = null;
        StudentSession.CurrentAccount = null;
        LogoutRequested?.Invoke(this, EventArgs.Empty);
    }
}
