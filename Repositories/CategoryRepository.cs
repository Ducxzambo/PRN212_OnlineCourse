using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class CategoryRepository : ICategoryRepository
{
    public async Task<List<Category>> GetAllAsync()
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
    }
}
