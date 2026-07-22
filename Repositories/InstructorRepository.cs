using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class InstructorRepository : IInstructorRepository
{
    public async Task<Instructor?> GetByEmailAsync(string email)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Instructors
            .AsNoTracking()
            .Include(i => i.Account)
            .FirstOrDefaultAsync(i => i.Account.Email.ToLower() == email.ToLower());
    }

    public async Task<Instructor?> GetByIdAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Instructors.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<Instructor>> GetAllAsync()
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Instructors
            .AsNoTracking()
            .Include(i => i.Account)
            .OrderBy(i => i.Account.FullName)
            .ToListAsync();
    }

    public async Task<int> GetCourseCountAsync(int instructorId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Courses.CountAsync(c => c.InstructorId == instructorId);
    }

    public async Task<int> AddAsync(Instructor instructor)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Instructors.Add(instructor);
        await context.SaveChangesAsync();
        return instructor.Id;
    }

    public async Task UpdateAsync(Instructor instructor)
    {
        using var context = new OnlineCourseManagementDbContext();
        var existing = await context.Instructors.FindAsync(instructor.Id);
        if (existing == null) return;

        existing.Phone = instructor.Phone;

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        var instructor = await context.Instructors.FindAsync(id);
        if (instructor != null)
        {
            context.Instructors.Remove(instructor);
            await context.SaveChangesAsync();
        }
    }
}
