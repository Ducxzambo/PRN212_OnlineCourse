using DataAccess.Models;

namespace Repositories;

public interface IInstructorRepository
{
    Task<Instructor?> GetByEmailAsync(string email);
    Task<Instructor?> GetByIdAsync(int id);
    Task<List<Instructor>> GetAllAsync();
    Task<int> GetCourseCountAsync(int instructorId);
    Task<int> AddAsync(Instructor instructor);
    Task UpdateAsync(Instructor instructor);
    Task DeleteAsync(int id);
}
