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

    public async Task<Category?> GetByIdAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Categories.AnyAsync(c =>
            c.Name.ToLower() == name.ToLower() && (!excludeId.HasValue || c.Id != excludeId.Value));
    }

    public async Task<int> GetCourseCountAsync(int categoryId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Courses.CountAsync(c => c.CategoryId == categoryId);
    }

    public async Task AddAsync(Category category)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Categories.Add(category);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        using var context = new OnlineCourseManagementDbContext();
        var existing = await context.Categories.FindAsync(category.Id);
        if (existing == null) return;

        existing.Name = category.Name;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        var category = await context.Categories.FindAsync(id);
        if (category != null)
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
        }
    }
}
