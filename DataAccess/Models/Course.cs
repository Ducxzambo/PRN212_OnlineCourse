using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Course
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int DurationHours { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CategoryId { get; set; }

    public int InstructorId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual Instructor Instructor { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
