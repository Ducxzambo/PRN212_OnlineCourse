using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;

namespace Presentation.ViewModels;

public class MainViewModel : ViewModelBase
{
    private object? _currentViewModel;

    private readonly CourseListViewModel _courseListViewModel;
    private readonly StudentListViewModel _studentListViewModel;

    public object? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public string InstructorName => InstructorSession.Current?.Account?.FullName ?? "";

    public ICommand NavigateCoursesCommand { get; }
    public ICommand NavigateStudentsCommand { get; }
    public ICommand OpenProfileCommand { get; }
    public ICommand LogoutCommand { get; }

    /// <summary>Raised when the instructor logs out - the shell view closes and shows the login window again.</summary>
    public event EventHandler? LogoutRequested;
    public event EventHandler? ProfileRequested;

    public MainViewModel()
    {
        _courseListViewModel = new CourseListViewModel(OpenLessons);
        _studentListViewModel = new StudentListViewModel();

        NavigateCoursesCommand = new RelayCommand(() => CurrentViewModel = _courseListViewModel);
        NavigateStudentsCommand = new RelayCommand(() => CurrentViewModel = _studentListViewModel);
        OpenProfileCommand = new RelayCommand(() => ProfileRequested?.Invoke(this, EventArgs.Empty));
        LogoutCommand = new RelayCommand(Logout);

        CurrentViewModel = _courseListViewModel;

        _courseListViewModel.Load();
        _studentListViewModel.Load();
    }

    private void OpenLessons(Course course)
    {
        var lessonListViewModel = new LessonListViewModel(course, () => CurrentViewModel = _courseListViewModel);
        CurrentViewModel = lessonListViewModel;
        lessonListViewModel.Load();
    }

    private void Logout()
    {
        InstructorSession.Current = null;
        LogoutRequested?.Invoke(this, EventArgs.Empty);
    }
}

