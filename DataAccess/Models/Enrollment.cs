using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Enrollment
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public DateTime EnrollDate { get; set; }

    public int Status { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
