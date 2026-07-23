using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Impl;

public class AccountRepository : IAccountRepository
{
    public  List<Account> GetAll()
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Accounts
            .Include(a => a.Instructor)
            .Include(a => a.Student)
            .OrderBy(a => a.FullName)
            .AsNoTracking()
            .ToList();
    }

    public  Account? GetById(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Accounts
            .Include(a => a.Instructor)
            .Include(a => a.Student)
            .AsNoTracking()
            .FirstOrDefault(a => a.Id == id);
    }

    public  Account? GetByEmail(string email)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Accounts
            .Include(a => a.Instructor)
            .Include(a => a.Student)
            .AsNoTracking()
            .FirstOrDefault(a => a.Email.ToLower() == email.ToLower());
    }

    public  void Add(Account account)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Accounts.Add(account);
        context.SaveChanges();
    }

    public  void Update(Account account)
    {
        using var context = new OnlineCourseManagementDbContext();
        var existing = context.Accounts.Find(account.Id);
        if (existing == null) return;

        existing.FullName = account.FullName;
        existing.Email = account.Email;
        existing.Password = account.Password;
        existing.IsActive = account.IsActive;
        // Role, InstructorId and StudentId are intentionally not changed after creation -
        // switching an account's role is out of scope; delete and recreate instead.

        context.SaveChanges();
    }

    public  void Delete(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        var account = context.Accounts.Find(id);
        if (account != null)
        {
            context.Accounts.Remove(account);
            context.SaveChanges();
        }
    }
}

