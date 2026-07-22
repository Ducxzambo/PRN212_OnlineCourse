using DataAccess.Models;

namespace Repositories;

public interface IEnrollmentRepository
{
    /// <summary>All enrollments for courses taught by the given instructor (the instructor's student roster).</summary>
    Task<List<Enrollment>> GetByInstructorAsync(int instructorId);

    /// <summary>Full enrollment history for one student, across every course in the system.</summary>
    Task<List<Enrollment>> GetByStudentAsync(int studentId);

    Task<Enrollment?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int studentId, int courseId);
    Task AddAsync(Enrollment enrollment);
    Task<bool> UpdateStatusAsync(int enrollmentId, int newStatus);
    Task<bool> UpdateProgressAsync(int enrollmentId, decimal progress);
    Task DeleteAsync(int id);

    /// <summary>Number of enrollments per course, used to rank courses by popularity.</summary>
    Task<Dictionary<int, int>> GetEnrollmentCountsByCourseAsync();
}
