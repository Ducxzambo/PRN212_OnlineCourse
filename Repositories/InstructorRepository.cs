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
            .FirstOrDefaultAsync(i => i.Email.ToLower() == email.ToLower());
    }

    public async Task<Instructor?> GetByIdAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Instructors.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<Instructor>> GetAllAsync()
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Instructors.AsNoTracking().OrderBy(i => i.FullName).ToListAsync();
    }
}
