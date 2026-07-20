using DataAccess.Models;

namespace Services;

public interface IStudentService
{
    /// <summary>Roster of every enrollment across all courses taught by this instructor.</summary>
    Task<List<Enrollment>> GetRosterByInstructorAsync(int instructorId);

    /// <summary>Full cross-course enrollment history for one student.</summary>
    Task<List<Enrollment>> GetEnrollmentHistoryAsync(int studentId);

    Task<(bool Success, string? Error)> UpdateEnrollmentStatusAsync(int enrollmentId, int newStatus);
    Task<(bool Success, string? Error)> RemoveEnrollmentAsync(int enrollmentId);

    /// <summary>Enrolls a student (creating the Student record if the email isn't known yet) into a course.</summary>
    Task<(bool Success, string? Error)> EnrollStudentAsync(int courseId, string fullName, string email, string? phone);

    /// <summary>Enrolls an already-known student (by Id) into a course - used by the recommendation screen.</summary>
    Task<(bool Success, string? Error)> EnrollExistingStudentAsync(int studentId, int courseId);
}
