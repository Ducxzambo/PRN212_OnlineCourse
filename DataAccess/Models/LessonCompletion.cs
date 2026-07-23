namespace DataAccess.Models;

/// <summary>Stores the lessons completed by a student.  Enrollment.Progress is calculated from these rows.</summary>
public class LessonCompletion
{
    public int Id { get; set; }
    public int EnrollmentId { get; set; }
    public int LessonId { get; set; }
    public DateTime CompletedDate { get; set; }
    public Enrollment Enrollment { get; set; } = null!;
    public Lesson Lesson { get; set; } = null!;
}
