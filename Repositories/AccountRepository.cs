using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class AccountRepository : IAccountRepository
{
    public async Task<List<Account>> GetAllAsync()
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Accounts
            .Include(a => a.Instructor)
            .Include(a => a.Student)
            .OrderBy(a => a.FullName)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Account?> GetByIdAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Accounts
            .Include(a => a.Instructor)
            .Include(a => a.Student)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Account?> GetByEmailAsync(string email)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Accounts
            .Include(a => a.Instructor)
            .Include(a => a.Student)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email.ToLower() == email.ToLower());
    }

    public async Task AddAsync(Account account)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Account account)
    {
        using var context = new OnlineCourseManagementDbContext();
        var existing = await context.Accounts.FindAsync(account.Id);
        if (existing == null) return;

        existing.FullName = account.FullName;
        existing.Email = account.Email;
        existing.IsActive = account.IsActive;
        // Role, InstructorId and StudentId are intentionally not changed after creation -
        // switching an account's role is out of scope; delete and recreate instead.

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        var account = await context.Accounts.FindAsync(id);
        if (account != null)
        {
            context.Accounts.Remove(account);
            await context.SaveChangesAsync();
        }
    }
}
