using DataAccess.Models;
using Repositories;

namespace Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public Task<List<Category>> GetAllAsync() => _categoryRepository.GetAllAsync();

    public async Task<(bool Success, string? Error)> CreateCategoryAsync(Category category)
    {
        var error = Validate(category);
        if (error != null) return (false, error);

        if (await _categoryRepository.ExistsByNameAsync(category.Name))
            return (false, "Tên danh mục đã tồn tại.");

        await _categoryRepository.AddAsync(category);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateCategoryAsync(Category category)
    {
        var error = Validate(category);
        if (error != null) return (false, error);

        if (await _categoryRepository.ExistsByNameAsync(category.Name, category.Id))
            return (false, "Tên danh mục đã tồn tại.");

        await _categoryRepository.UpdateAsync(category);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteCategoryAsync(int categoryId)
    {
        var courseCount = await _categoryRepository.GetCourseCountAsync(categoryId);
        if (courseCount > 0)
            return (false, "Không thể xóa danh mục đang có khóa học sử dụng.");

        await _categoryRepository.DeleteAsync(categoryId);
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
