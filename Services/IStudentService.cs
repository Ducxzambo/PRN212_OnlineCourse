using DataAccess.Models;

namespace Services;

public interface IStudentService
{
    /// <summary>Roster of every enrollment across all courses taught by this instructor.</summary>
    List<Enrollment> GetRosterByInstructor(int instructorId);

    /// <summary>Full cross-course enrollment history for one student.</summary>
    List<Enrollment> GetEnrollmentHistory(int studentId);

    (bool Success, string? Error) UpdateEnrollmentStatus(int enrollmentId, int newStatus);
    (bool Success, string? Error) RemoveEnrollment(int enrollmentId);

    /// <summary>Enrolls a student (creating the Student record if the email isn't known yet) into a course.</summary>
    (bool Success, string? Error) EnrollStudent(int courseId, string fullName, string email, string? phone);

    /// <summary>Enrolls an already-known student (by Id) into a course - used by the recommendation screen.</summary>
    (bool Success, string? Error) EnrollExistingStudent(int studentId, int courseId);
    (bool Success, string? Error) CompleteLesson(int enrollmentId, int lessonId);
    (bool Success, string? Error) SetLessonCompletion(int enrollmentId, int lessonId, bool isCompleted);
    List<int> GetCompletedLessonIds(int enrollmentId);
}

