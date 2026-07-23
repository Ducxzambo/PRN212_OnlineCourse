using DataAccess.Models;

namespace Services;

public interface ICategoryService
{
    List<Category> GetAll();
    (bool Success, string? Error) CreateCategory(Category category);
    (bool Success, string? Error) UpdateCategory(Category category);
    (bool Success, string? Error) DeleteCategory(int categoryId);
}

