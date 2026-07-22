using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Instructor
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string? Phone { get; set; }

    public string? Specialization { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}

