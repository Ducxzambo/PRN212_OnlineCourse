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
    public  List<T> GetAll()
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Set<T>().AsNoTracking().ToList();
    }

    public  T? GetById(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Set<T>().Find(id);
    }

    public List<T> Find(Expression<Func<T, bool>> predicate)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Set<T>().AsNoTracking().Where(predicate).ToList();
    }

    public  void Add(T entity)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Set<T>().Add(entity);
        context.SaveChanges();
    }

    public  void Update(T entity)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Set<T>().Update(entity);
        context.SaveChanges();
    }

    public  void Delete(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        var entity = context.Set<T>().Find(id);
        if (entity != null)
        {
            context.Set<T>().Remove(entity);
            context.SaveChanges();
        }
    }
}

