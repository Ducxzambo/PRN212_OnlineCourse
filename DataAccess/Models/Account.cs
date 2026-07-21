using System;
using System.Collections.Generic;

namespace DataAccess.Models;

/// <summary>
/// Login/role record. Role: 0 = Instructor, 1 = Admin, 2 = Student.
/// When Role = Instructor, InstructorId points to the matching Instructors row
/// (created together by AccountService so Courses.InstructorId keeps working).
/// When Role = Student, StudentId points to the matching Students row
/// (created together by AccountService so Enrollments.StudentId keeps working).
/// When Role = Admin, both InstructorId and StudentId are null.
/// </summary>
public partial class Account
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int Role { get; set; }

    public bool IsActive { get; set; } = true;

    public int? InstructorId { get; set; }

    public int? StudentId { get; set; }

    public virtual Instructor? Instructor { get; set; }

    public virtual Student? Student { get; set; }
}
