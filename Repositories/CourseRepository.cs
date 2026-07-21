using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class CourseRepository : ICourseRepository
{
    public async Task<List<Course>> GetByInstructorAsync(int instructorId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Courses
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .Where(c => c.InstructorId == instructorId)
            .OrderByDescending(c => c.CreatedDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Course?> GetByIdWithDetailsAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Courses
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Course>> GetAllWithDetailsAsync()
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Courses
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Course>> GetByCategoryAsync(int categoryId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Courses
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Enrollments)
            .Where(c => c.CategoryId == categoryId)
            .OrderByDescending(c => c.CreatedDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Course course)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Courses.Add(course);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        using var context = new OnlineCourseManagementDbContext();
        var existing = await context.Courses.FindAsync(course.Id);
        if (existing == null) return;

        // Copy scalar fields only - never reuse a populated navigation graph here,
        // otherwise EF would also try to attach Category/Instructor/Lessons/Enrollments.
        existing.Title = course.Title;
        existing.Description = course.Description;
        existing.Price = course.Price;
        existing.DurationHours = course.DurationHours;
        existing.CategoryId = course.CategoryId;

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        var course = await context.Courses.FindAsync(id);
        if (course != null)
        {
            context.Courses.Remove(course);
            await context.SaveChangesAsync();
        }
    }
}
