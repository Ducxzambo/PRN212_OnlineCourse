using DataAccess.Models;

namespace Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task<int> GetCourseCountAsync(int categoryId);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int id);
}
