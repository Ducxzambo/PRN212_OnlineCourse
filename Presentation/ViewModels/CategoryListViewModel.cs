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

    public ObservableCollection<Category> Categories { get; } = new();

    /// <summary>Courses belonging to the currently selected category - shown in the grid below.</summary>
    public ObservableCollection<Course> CoursesInCategory { get; } = new();

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
                _ = LoadCoursesInCategoryAsync();
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
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedCategory != null);
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
    }

    public async Task LoadAsync()
    {
        var previousSelectedId = SelectedCategory?.Id;

        _allCategories = await AppServices.CategoryService.GetAllAsync();
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

    private async Task LoadCoursesInCategoryAsync()
    {
        CoursesInCategory.Clear();
        if (SelectedCategory == null) return;

        var courses = await AppServices.CourseService.GetCoursesByCategoryAsync(SelectedCategory.Id);
        foreach (var course in courses) CoursesInCategory.Add(course);
    }

    private void Add()
    {
        var window = new CategoryEditWindow(new CategoryEditViewModel(null));
        if (window.ShowDialog() == true) _ = LoadAsync();
    }

    private void Edit()
    {
        if (SelectedCategory == null) return;

        var window = new CategoryEditWindow(new CategoryEditViewModel(SelectedCategory));
        if (window.ShowDialog() == true) _ = LoadAsync();
    }

    private async Task DeleteAsync()
    {
        if (SelectedCategory == null) return;

        var confirm = MessageBox.Show(
            $"Xóa danh mục '{SelectedCategory.Name}'?", "Xác nhận xóa",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        var (success, error) = await AppServices.CategoryService.DeleteCategoryAsync(SelectedCategory.Id);
        if (!success)
        {
            MessageBox.Show(error, "Không thể xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        await LoadAsync();
    }
}
