using DataAccess.Models;
using Repositories;

namespace Services.Impl;

public class StudentService : IStudentService
{
    private readonly IEnrollmentRepository _enrollments;
    private readonly IStudentRepository _students;
    public StudentService(IEnrollmentRepository enrollments, IStudentRepository students) { _enrollments = enrollments; _students = students; }
    public List<Enrollment> GetRosterByInstructor(int instructorId) => _enrollments.GetByInstructor(instructorId);
    public List<Enrollment> GetEnrollmentHistory(int studentId) => _enrollments.GetByStudent(studentId);
    public  (bool Success, string? Error) UpdateEnrollmentStatus(int enrollmentId, int newStatus)
        => _enrollments.UpdateStatus(enrollmentId, newStatus) ? (true, null) : (false, "Không tìm thấy lượt đăng ký.");
    public  (bool Success, string? Error) RemoveEnrollment(int enrollmentId) { _enrollments.Delete(enrollmentId); return (true, null); }
    public (bool Success, string? Error) EnrollStudent(int courseId, string fullName, string email, string? phone)
        => (false, (string?)"Instructor không được tạo hoặc ghi danh Student.");
    public  (bool Success, string? Error) EnrollExistingStudent(int studentId, int courseId)
    {
        if (_enrollments.Exists(studentId, courseId)) return (false, "Bạn đã đăng ký khóa học này.");
        _enrollments.Add(new Enrollment { StudentId = studentId, CourseId = courseId, EnrollDate = DateTime.Now, Status = 0, Progress = 0 });
        return (true, null);
    }
    public  (bool Success, string? Error) CompleteLesson(int enrollmentId, int lessonId)
        => _enrollments.CompleteLesson(enrollmentId, lessonId) ? (true, null) : (false, "Không thể cập nhật bài học.");
    public (bool Success, string? Error) SetLessonCompletion(int enrollmentId, int lessonId, bool isCompleted)
        => _enrollments.SetLessonCompletion(enrollmentId, lessonId, isCompleted)
            ? (true, null)
            : (false, "Không thể cập nhật bài học.");
    public List<int> GetCompletedLessonIds(int enrollmentId) => _enrollments.GetCompletedLessonIds(enrollmentId);
}

