namespace Services.Models;

/// <summary>Mirrors the int Status column on dbo.Enrollments.</summary>
public enum EnrollmentStatus
{
    Registered = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}

