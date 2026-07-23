using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;
using Presentation.Views;

namespace Presentation.ViewModels;

public class CategoryListViewModel : ViewModelBase
{
    private List<Category> _allCategories = new();
    private string _searchText = "";
    private Category? _selectedCategory;
    private Course? _selectedCourseInCategory;

    public ObservableCollection<Category> Categories { get; } = new();

    /// <summary>Courses belonging to the currently selected category - shown in the grid below.</summary>
    public ObservableCollection<Course> CoursesInCategory { get; } = new();
    public ObservableCollection<Lesson> LessonsInSelectedCourse { get; } = new();

    public string SearchText
    {
        get => _searchText;
        set { if (SetProperty(ref _searchText, value)) ApplyFilter(); }
    }

    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
                LoadCoursesInCategory();
        }
    }

    public Course? SelectedCourseInCategory
    {
        get => _selectedCourseInCategory;
        set
        {
            if (SetProperty(ref _selectedCourseInCategory, value))
                LoadLessonsInSelectedCourse();
        }
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand RefreshCommand { get; }

    public CategoryListViewModel()
    {
        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand(Edit, () => SelectedCategory != null);
        DeleteCommand = new RelayCommand(Delete, () => SelectedCategory != null);
        RefreshCommand = new RelayCommand(Load);
    }

    public  void Load()
    {
        var previousSelectedId = SelectedCategory?.Id;

        _allCategories = AppServices.CategoryService.GetAll();
        ApplyFilter();

        if (previousSelectedId.HasValue)
            SelectedCategory = Categories.FirstOrDefault(c => c.Id == previousSelectedId.Value);
    }

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allCategories
            : _allCategories.Where(c => c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

        Categories.Clear();
        foreach (var category in filtered) Categories.Add(category);
    }

    private  void LoadCoursesInCategory()
    {
        CoursesInCategory.Clear();
        LessonsInSelectedCourse.Clear();
        SelectedCourseInCategory = null;
        if (SelectedCategory == null) return;

        var courses = AppServices.CourseService.GetCoursesByCategory(SelectedCategory.Id);
        foreach (var course in courses) CoursesInCategory.Add(course);
        SelectedCourseInCategory = CoursesInCategory.FirstOrDefault();
    }

    private void LoadLessonsInSelectedCourse()
    {
        LessonsInSelectedCourse.Clear();
        if (SelectedCourseInCategory == null) return;

        foreach (var lesson in AppServices.LessonService.GetLessonsByCourse(SelectedCourseInCategory.Id))
            LessonsInSelectedCourse.Add(lesson);
    }

    private void Add()
    {
        var window = new CategoryEditWindow(new CategoryEditViewModel(null));
        if (window.ShowDialog() == true) Load();
    }

    private void Edit()
    {
        if (SelectedCategory == null) return;

        var window = new CategoryEditWindow(new CategoryEditViewModel(SelectedCategory));
        if (window.ShowDialog() == true) Load();
    }

    private  void Delete()
    {
        if (SelectedCategory == null) return;

        var confirm = MessageBox.Show(
            $"Xóa danh mục '{SelectedCategory.Name}'?", "Xác nhận xóa",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        var (success, error) = AppServices.CategoryService.DeleteCategory(SelectedCategory.Id);
        if (!success)
        {
            MessageBox.Show(error, "Không thể xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Load();
    }
}

