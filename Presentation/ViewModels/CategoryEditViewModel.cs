using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;

namespace Presentation.ViewModels;

public class CategoryEditViewModel : ViewModelBase
{
    private readonly int? _categoryId;
    private string _name = "";
    private string? _errorMessage;

    public bool IsEdit => _categoryId.HasValue;
    public string WindowTitle => IsEdit ? "Sửa danh mục" : "Thêm danh mục mới";

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public ICommand SaveCommand { get; }

    public event EventHandler? SaveSucceeded;

    public CategoryEditViewModel(Category? category)
    {
        if (category != null)
        {
            _categoryId = category.Id;
            Name = category.Name;
        }

        SaveCommand = new AsyncRelayCommand(SaveAsync);
    }

    private async Task SaveAsync()
    {
        ErrorMessage = null;

        var category = new Category
        {
            Id = _categoryId ?? 0,
            Name = Name?.Trim() ?? ""
        };

        var (success, error) = IsEdit
            ? await AppServices.CategoryService.UpdateCategoryAsync(category)
            : await AppServices.CategoryService.CreateCategoryAsync(category);

        if (!success)
        {
            ErrorMessage = error;
            return;
        }

        SaveSucceeded?.Invoke(this, EventArgs.Empty);
    }
}
