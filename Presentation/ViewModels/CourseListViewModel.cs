using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;
using Presentation.Views;

namespace Presentation.ViewModels;

public class CourseListViewModel : ViewModelBase
{
    private readonly Action<Course> _onManageLessons;
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

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ManageLessonsCommand { get; }
    public ICommand RefreshCommand { get; }

    public CourseListViewModel(Action<Course> onManageLessons)
    {
        _onManageLessons = onManageLessons;

        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand(Edit, () => SelectedCourse != null);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedCourse != null);
        ManageLessonsCommand = new RelayCommand(ManageLessons, () => SelectedCourse != null);
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
    }

    public async Task LoadAsync()
    {
        if (InstructorSession.Current == null) return;

        var previousSelectedId = SelectedCourse?.Id;

        _allCourses = await AppServices.CourseService.GetCoursesByInstructorAsync(InstructorSession.Current.Id);
        ApplyFilter();

        if (previousSelectedId.HasValue)
            SelectedCourse = Courses.FirstOrDefault(c => c.Id == previousSelectedId.Value);
    }

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allCourses
            : _allCourses.Where(c =>
                    c.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    (c.Category != null && c.Category.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
                .ToList();

        Courses.Clear();
        foreach (var course in filtered) Courses.Add(course);
    }

    private void Add()
    {
        var window = new CourseEditWindow(new CourseEditViewModel(null));
        if (window.ShowDialog() == true) _ = LoadAsync();
    }

    private void Edit()
    {
        if (SelectedCourse == null) return;

        var window = new CourseEditWindow(new CourseEditViewModel(SelectedCourse));
        if (window.ShowDialog() == true) _ = LoadAsync();
    }

    private async Task DeleteAsync()
    {
        if (SelectedCourse == null) return;

        var confirm = MessageBox.Show(
            $"Xóa khóa học '{SelectedCourse.Title}'? Hành động này không thể hoàn tác.",
            "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        var (success, error) = await AppServices.CourseService.DeleteCourseAsync(SelectedCourse.Id);
        if (!success)
        {
            MessageBox.Show(error, "Không thể xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        await LoadAsync();
    }

    private void ManageLessons()
    {
        if (SelectedCourse != null) _onManageLessons(SelectedCourse);
    }
}
