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
            .FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());
    }

    public async Task AddAsync(Student student)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Students.Add(student);
        await context.SaveChangesAsync();
    }
}
