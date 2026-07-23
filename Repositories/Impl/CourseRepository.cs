using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Impl;

public class CourseRepository : ICourseRepository
{
    public  List<Course> GetAll()
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Courses.Include(c => c.Category).Include(c => c.Instructor).ThenInclude(i => i.Account)
            .OrderBy(c => c.Title).AsNoTracking().ToList();
    }
    public  List<Course> GetByInstructor(int instructorId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Courses
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .Where(c => c.InstructorId == instructorId)
            .OrderByDescending(c => c.CreatedDate)
            .AsNoTracking()
            .ToList();
    }

    public  Course? GetByIdWithDetails(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Courses
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .AsNoTracking()
            .FirstOrDefault(c => c.Id == id);
    }

    public  List<Course> GetAllWithDetails()
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Courses
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .AsNoTracking()
            .ToList();
    }

    public  List<Course> GetByCategory(int categoryId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Courses
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Enrollments)
            .Where(c => c.CategoryId == categoryId)
            .OrderByDescending(c => c.CreatedDate)
            .AsNoTracking()
            .ToList();
    }

    public  void Add(Course course)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Courses.Add(course);
        context.SaveChanges();
    }

    public  void Update(Course course)
    {
        using var context = new OnlineCourseManagementDbContext();
        var existing = context.Courses.Find(course.Id);
        if (existing == null) return;

        // Copy scalar fields only - never reuse a populated navigation graph here,
        // otherwise EF would also try to attach Category/Instructor/Lessons/Enrollments.
        existing.Title = course.Title;
        existing.Description = course.Description;
        existing.Price = course.Price;
        existing.DurationHours = course.DurationHours;
        existing.CategoryId = course.CategoryId;

        context.SaveChanges();
    }

    public  void Delete(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        var course = context.Courses.Find(id);
        if (course != null)
        {
            context.Courses.Remove(course);
            context.SaveChanges();
        }
    }
}

