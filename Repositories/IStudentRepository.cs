using DataAccess.Models;

namespace Repositories;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int id);
    Task<Student?> GetByEmailAsync(string email);
    Task<int> AddAsync(Student student);
    Task UpdateAsync(Student student);
    Task<int> GetEnrollmentCountAsync(int studentId);

    /// <summary>Get student with enrollments and course details loaded.</summary>
    Task<Student?> GetByIdWithEnrollmentsAsync(int id);
}
