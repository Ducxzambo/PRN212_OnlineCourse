using DataAccess.Models;

namespace Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<(bool Success, string? Error)> CreateCategoryAsync(Category category);
    Task<(bool Success, string? Error)> UpdateCategoryAsync(Category category);
    Task<(bool Success, string? Error)> DeleteCategoryAsync(int categoryId);
}
