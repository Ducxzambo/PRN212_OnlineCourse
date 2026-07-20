using DataAccess.Models;
using Repositories;

namespace Services;

public class StudentService : IStudentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;

    public StudentService(IEnrollmentRepository enrollmentRepository, IStudentRepository studentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
    }

    public Task<List<Enrollment>> GetRosterByInstructorAsync(int instructorId)
        => _enrollmentRepository.GetByInstructorAsync(instructorId);

    public Task<List<Enrollment>> GetEnrollmentHistoryAsync(int studentId)
        => _enrollmentRepository.GetByStudentAsync(studentId);

    public async Task<(bool Success, string? Error)> UpdateEnrollmentStatusAsync(int enrollmentId, int newStatus)
    {
        var ok = await _enrollmentRepository.UpdateStatusAsync(enrollmentId, newStatus);
        return ok ? (true, null) : (false, "Không tìm thấy lượt đăng ký.");
    }

    public async Task<(bool Success, string? Error)> RemoveEnrollmentAsync(int enrollmentId)
    {
        await _enrollmentRepository.DeleteAsync(enrollmentId);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> EnrollStudentAsync(int courseId, string fullName, string email, string? phone)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return (false, "Họ tên học viên không được để trống.");
        if (string.IsNullOrWhiteSpace(email))
            return (false, "Email học viên không được để trống.");

        var student = await _studentRepository.GetByEmailAsync(email.Trim());
        if (student == null)
        {
            student = new Student
            {
                FullName = fullName.Trim(),
                Email = email.Trim(),
                Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim()
            };
            await _studentRepository.AddAsync(student);

            // AddAsync ran in its own DbContext, so re-fetch to get the generated Id.
            student = await _studentRepository.GetByEmailAsync(email.Trim());
            if (student == null)
                return (false, "Không thể tạo học viên mới.");
        }

        return await EnrollExistingStudentAsync(student.Id, courseId);
    }

    public async Task<(bool Success, string? Error)> EnrollExistingStudentAsync(int studentId, int courseId)
    {
        var alreadyEnrolled = await _enrollmentRepository.ExistsAsync(studentId, courseId);
        if (alreadyEnrolled)
            return (false, "Học viên đã đăng ký khóa học này rồi.");

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrollDate = DateTime.Now,
            Status = (int)Models.EnrollmentStatus.Registered
        };

        await _enrollmentRepository.AddAsync(enrollment);
        return (true, null);
    }
}
