using DataAccess.Models;

namespace Repositories;

public interface IEnrollmentRepository
{
    /// <summary>All enrollments for courses taught by the given instructor (the instructor's student roster).</summary>
    List<Enrollment> GetByInstructor(int instructorId);

    /// <summary>Full enrollment history for one student, across every course in the system.</summary>
    List<Enrollment> GetByStudent(int studentId);

    Enrollment? GetById(int id);
    bool Exists(int studentId, int courseId);
    void Add(Enrollment enrollment);
    bool UpdateStatus(int enrollmentId, int newStatus);
    bool CompleteLesson(int enrollmentId, int lessonId);
    bool SetLessonCompletion(int enrollmentId, int lessonId, bool isCompleted);
    List<int> GetCompletedLessonIds(int enrollmentId);
    void Delete(int id);

    /// <summary>Number of enrollments per course, used to rank courses by popularity.</summary>
    Dictionary<int, int> GetEnrollmentCountsByCourse();
}

