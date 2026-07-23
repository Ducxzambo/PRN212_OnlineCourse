using DataAccess.Models;
using Repositories;

namespace Services.Impl;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public List<Category> GetAll() => _categoryRepository.GetAll();

    public  (bool Success, string? Error) CreateCategory(Category category)
    {
        var error = Validate(category);
        if (error != null) return (false, error);

        if (_categoryRepository.ExistsByName(category.Name))
            return (false, "Tên danh mục đã tồn tại.");

        _categoryRepository.Add(category);
        return (true, null);
    }

    public  (bool Success, string? Error) UpdateCategory(Category category)
    {
        var error = Validate(category);
        if (error != null) return (false, error);

        if (_categoryRepository.ExistsByName(category.Name, category.Id))
            return (false, "Tên danh mục đã tồn tại.");

        _categoryRepository.Update(category);
        return (true, null);
    }

    public  (bool Success, string? Error) DeleteCategory(int categoryId)
    {
        var courseCount = _categoryRepository.GetCourseCount(categoryId);
        if (courseCount > 0)
            return (false, "Không thể xóa danh mục đang có khóa học sử dụng.");

        _categoryRepository.Delete(categoryId);
        return (true, null);
    }

    private static string? Validate(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
            return "Tên danh mục không được để trống.";
        if (category.Name.Length > 100)
            return "Tên danh mục không được vượt quá 100 ký tự.";

        return null;
    }
}

