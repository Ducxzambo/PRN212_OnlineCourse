using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class StudentRepository : IStudentRepository
{
    public async Task<Student?> GetByIdAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Students
            .AsNoTracking()
            .Include(s => s.Account)
            .FirstOrDefaultAsync(s => s.Account.Email.ToLower() == email.ToLower());
    }

    public async Task<int> AddAsync(Student student)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Students.Add(student);
        await context.SaveChangesAsync();
        return student.Id;
    }

    public async Task UpdateAsync(Student student)
    {
        using var context = new OnlineCourseManagementDbContext();
        var existing = await context.Students.FindAsync(student.Id);
        if (existing == null) return;

        existing.Phone = student.Phone;

        await context.SaveChangesAsync();
    }

    public async Task<int> GetEnrollmentCountAsync(int studentId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Enrollments.CountAsync(e => e.StudentId == studentId);
    }

    public async Task<Student?> GetByIdWithEnrollmentsAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.Instructor)
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}
