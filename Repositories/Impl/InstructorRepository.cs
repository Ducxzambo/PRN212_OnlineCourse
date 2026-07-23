using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Impl;

public class InstructorRepository : IInstructorRepository
{
    public  Instructor? GetByEmail(string email)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Instructors.Include(i => i.Account)
            .AsNoTracking()
            .FirstOrDefault(i => i.Account.Email.ToLower() == email.ToLower());
    }

    public  Instructor? GetById(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Instructors.Include(i => i.Account).AsNoTracking().FirstOrDefault(i => i.Id == id);
    }

    public  List<Instructor> GetAll()
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Instructors.Include(i => i.Account).AsNoTracking().OrderBy(i => i.Account.FullName).ToList();
    }

    public  int GetCourseCount(int instructorId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Courses.Count(c => c.InstructorId == instructorId);
    }

    public  int Add(Instructor instructor)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Instructors.Add(instructor);
        context.SaveChanges();
        return instructor.Id;
    }

    public  void Update(Instructor instructor)
    {
        using var context = new OnlineCourseManagementDbContext();
        var existing = context.Instructors.Find(instructor.Id);
        if (existing == null) return;

        existing.Phone = instructor.Phone;
        existing.Specialization = instructor.Specialization;

        context.SaveChanges();
    }

    public  void Delete(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        var instructor = context.Instructors.Find(id);
        if (instructor != null)
        {
            context.Instructors.Remove(instructor);
            context.SaveChanges();
        }
    }
}

