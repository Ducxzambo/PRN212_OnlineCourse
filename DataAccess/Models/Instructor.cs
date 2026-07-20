using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Instructor
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
