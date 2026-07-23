using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Student
{
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string FullName { get => Account?.FullName ?? ""; set { if (Account != null) Account.FullName = value; } }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string Email { get => Account?.Email ?? ""; set { if (Account != null) Account.Email = value; } }
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string? Phone { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
