using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Impl;

public class CategoryRepository : ICategoryRepository
{
    public  List<Category> GetAll()
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Categories.AsNoTracking().OrderBy(c => c.Name).ToList();
    }

    public  Category? GetById(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Categories.AsNoTracking().FirstOrDefault(c => c.Id == id);
    }

    public  bool ExistsByName(string name, int? excludeId = null)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Categories.Any(c =>
            c.Name.ToLower() == name.ToLower() && (!excludeId.HasValue || c.Id != excludeId.Value));
    }

    public  int GetCourseCount(int categoryId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Courses.Count(c => c.CategoryId == categoryId);
    }

    public  void Add(Category category)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Categories.Add(category);
        context.SaveChanges();
    }

    public  void Update(Category category)
    {
        using var context = new OnlineCourseManagementDbContext();
        var existing = context.Categories.Find(category.Id);
        if (existing == null) return;

        existing.Name = category.Name;
        context.SaveChanges();
    }

    public  void Delete(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        var category = context.Categories.Find(id);
        if (category != null)
        {
            context.Categories.Remove(category);
            context.SaveChanges();
        }
    }
}

