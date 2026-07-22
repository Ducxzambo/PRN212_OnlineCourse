using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;

namespace Presentation.ViewModels;

public class MainViewModel : ViewModelBase
{
    private object? _currentViewModel;

    private readonly CourseListViewModel _courseListViewModel;
    private readonly StudentListViewModel _studentListViewModel;
    private readonly RecommendationViewModel _recommendationViewModel;

    public object? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public string InstructorName => AccountSession.Current?.FullName ?? "";

    public ICommand NavigateCoursesCommand { get; }
    public ICommand NavigateStudentsCommand { get; }
    public ICommand NavigateRecommendationCommand { get; }
    public ICommand LogoutCommand { get; }

    /// <summary>Raised when the instructor logs out - the shell view closes and shows the login window again.</summary>
    public event EventHandler? LogoutRequested;

    public MainViewModel()
    {
        _courseListViewModel = new CourseListViewModel(OpenLessons);
        _studentListViewModel = new StudentListViewModel();
        _recommendationViewModel = new RecommendationViewModel();

        NavigateCoursesCommand = new RelayCommand(() => CurrentViewModel = _courseListViewModel);
        NavigateStudentsCommand = new RelayCommand(() => CurrentViewModel = _studentListViewModel);
        NavigateRecommendationCommand = new RelayCommand(() => CurrentViewModel = _recommendationViewModel);
        LogoutCommand = new RelayCommand(Logout);

        CurrentViewModel = _courseListViewModel;

        _ = _courseListViewModel.LoadAsync();
        _ = _studentListViewModel.LoadAsync();
        _ = _recommendationViewModel.LoadAsync();
    }

    private void OpenLessons(Course course)
    {
        var lessonListViewModel = new LessonListViewModel(course, () => CurrentViewModel = _courseListViewModel);
        CurrentViewModel = lessonListViewModel;
        _ = lessonListViewModel.LoadAsync();
    }

    private void Logout()
    {
        InstructorSession.Current = null;
        LogoutRequested?.Invoke(this, EventArgs.Empty);
    }
}
