using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Impl;

public class StudentRepository : IStudentRepository
{
    public  Student? GetById(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Students.Include(s => s.Account).AsNoTracking().FirstOrDefault(s => s.Id == id);
    }

    public  Student? GetByEmail(string email)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Students.Include(s => s.Account)
            .AsNoTracking()
            .FirstOrDefault(s => s.Account.Email.ToLower() == email.ToLower());
    }

    public  int Add(Student student)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Students.Add(student);
        context.SaveChanges();
        return student.Id;
    }

    public  void Update(Student student)
    {
        using var context = new OnlineCourseManagementDbContext();
        var existing = context.Students.Find(student.Id);
        if (existing == null) return;

        existing.Phone = student.Phone;
        existing.DateOfBirth = student.DateOfBirth;

        context.SaveChanges();
    }

    public  int GetEnrollmentCount(int studentId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Enrollments.Count(e => e.StudentId == studentId);
    }
}

