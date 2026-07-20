using DataAccess.Models;

namespace Repositories;

public interface ICourseRepository
{
    Task<List<Course>> GetByInstructorAsync(int instructorId);
    Task<Course?> GetByIdWithDetailsAsync(int id);
    Task<List<Course>> GetAllWithDetailsAsync();
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(int id);
}
