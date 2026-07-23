using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;

namespace Presentation.ViewModels;

public class CourseEditViewModel : ViewModelBase
{
    private readonly int? _courseId;
    private string _title = "";
    private string? _description;
    private decimal _price;
    private int _durationHours;
    private Category? _selectedCategory;
    private string? _errorMessage;

    public bool IsEdit => _courseId.HasValue;
    public string WindowTitle => IsEdit ? "Sửa khóa học" : "Thêm khóa học mới";

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public decimal Price
    {
        get => _price;
        set => SetProperty(ref _price, value);
    }

    public int DurationHours
    {
        get => _durationHours;
        set => SetProperty(ref _durationHours, value);
    }

    public ObservableCollection<Category> Categories { get; } = new();

    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public ICommand SaveCommand { get; }

    /// <summary>Raised after a successful save so the hosting Window can set DialogResult = true and close.</summary>
    public event EventHandler? SaveSucceeded;

    public CourseEditViewModel(Course? course)
    {
        if (course != null)
        {
            _courseId = course.Id;
            Title = course.Title;
            Description = course.Description;
            Price = course.Price;
            DurationHours = course.DurationHours;
        }

        SaveCommand = new RelayCommand(Save);
        LoadCategories(course?.CategoryId);
    }

    private  void LoadCategories(int? currentCategoryId)
    {
        var categories = AppServices.CourseService.GetCategories();
        Categories.Clear();
        foreach (var category in categories) Categories.Add(category);

        if (currentCategoryId.HasValue)
            SelectedCategory = Categories.FirstOrDefault(c => c.Id == currentCategoryId.Value);
    }

    private  void Save()
    {
        ErrorMessage = null;

        if (SelectedCategory == null)
        {
            ErrorMessage = "Vui lòng chọn danh mục.";
            return;
        }

        // A fresh, "clean" entity with only scalar fields set - navigation properties are
        // deliberately left untouched so the update only ever touches the Courses row itself.
        var course = new Course
        {
            Id = _courseId ?? 0,
            Title = Title?.Trim() ?? "",
            Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
            Price = Price,
            DurationHours = DurationHours,
            CategoryId = SelectedCategory.Id,
            InstructorId = InstructorSession.Current?.Id ?? 0
        };

        var (success, error) = IsEdit
            ? AppServices.CourseService.UpdateCourse(course)
            : AppServices.CourseService.CreateCourse(course);

        if (!success)
        {
            ErrorMessage = error;
            return;
        }

        SaveSucceeded?.Invoke(this, EventArgs.Empty);
    }
}

