using System.Collections.ObjectModel;
using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;

namespace Presentation.ViewModels;

/// <summary>Read-only course browser for administrators.</summary>
public class AdminCourseListViewModel : ViewModelBase
{
    private readonly Action<Course> _onViewLessons;
    private List<Course> _allCourses = new();
    private string _searchText = "";
    private Course? _selectedCourse;

    public ObservableCollection<Course> Courses { get; } = new();

    public string SearchText
    {
        get => _searchText;
        set { if (SetProperty(ref _searchText, value)) ApplyFilter(); }
    }

    public Course? SelectedCourse
    {
        get => _selectedCourse;
        set => SetProperty(ref _selectedCourse, value);
    }

    public ICommand ViewLessonsCommand { get; }
    public ICommand RefreshCommand { get; }

    public AdminCourseListViewModel(Action<Course> onViewLessons)
    {
        _onViewLessons = onViewLessons;
        ViewLessonsCommand = new RelayCommand(ViewLessons, () => SelectedCourse != null);
        RefreshCommand = new RelayCommand(Load);
    }

    public void Load()
    {
        var selectedId = SelectedCourse?.Id;
        _allCourses = AppServices.CourseService.GetAllCourses();
        ApplyFilter();
        if (selectedId.HasValue)
            SelectedCourse = Courses.FirstOrDefault(course => course.Id == selectedId.Value);
    }

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allCourses
            : _allCourses.Where(course =>
                course.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                (course.Category?.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (course.Instructor?.Account?.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));

        Courses.Clear();
        foreach (var course in filtered) Courses.Add(course);
    }

    private void ViewLessons()
    {
        if (SelectedCourse != null) _onViewLessons(SelectedCourse);
    }
}
