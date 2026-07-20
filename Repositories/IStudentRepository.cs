using DataAccess.Models;

namespace Repositories;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int id);
    Task<Student?> GetByEmailAsync(string email);
    Task AddAsync(Student student);
}
