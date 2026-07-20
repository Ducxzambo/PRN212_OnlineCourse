using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    public async Task<List<Enrollment>> GetByInstructorAsync(int instructorId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Where(e => e.Course.InstructorId == instructorId)
            .OrderByDescending(e => e.EnrollDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Enrollment>> GetByStudentAsync(int studentId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Enrollments
            .Include(e => e.Course)
                .ThenInclude(c => c.Category)
            .Include(e => e.Course)
                .ThenInclude(c => c.Instructor)
            .Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.EnrollDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Enrollment?> GetByIdAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> ExistsAsync(int studentId, int courseId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
    }

    public async Task AddAsync(Enrollment enrollment)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync();
    }

    public async Task<bool> UpdateStatusAsync(int enrollmentId, int newStatus)
    {
        using var context = new OnlineCourseManagementDbContext();
        var enrollment = await context.Enrollments.FindAsync(enrollmentId);
        if (enrollment == null) return false;

        enrollment.Status = newStatus;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task DeleteAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        var enrollment = await context.Enrollments.FindAsync(id);
        if (enrollment != null)
        {
            context.Enrollments.Remove(enrollment);
            await context.SaveChangesAsync();
        }
    }

    public async Task<Dictionary<int, int>> GetEnrollmentCountsByCourseAsync()
    {
        using var context = new OnlineCourseManagementDbContext();
        var grouped = await context.Enrollments
            .GroupBy(e => e.CourseId)
            .Select(g => new { CourseId = g.Key, Count = g.Count() })
            .ToListAsync();

        return grouped.ToDictionary(x => x.CourseId, x => x.Count);
    }
}
