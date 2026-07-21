using System.Linq.Expressions;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Common;

/// <summary>
/// Basic CRUD implementation shared by every entity repository.
/// Each method opens and disposes its own DbContext so the repository
/// itself is stateless and safe to keep around as a singleton.
/// </summary>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    public async Task<List<T>> GetAllAsync()
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Set<T>().FindAsync(id);
    }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Set<T>().AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Set<T>().Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        var entity = await context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
