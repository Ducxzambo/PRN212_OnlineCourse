using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Instructor
{
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string FullName { get => Account?.FullName ?? ""; set { if (Account != null) Account.FullName = value; } }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string Email { get => Account?.Email ?? ""; set { if (Account != null) Account.Email = value; } }
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string? Phone { get; set; }

    public string? Specialization { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
