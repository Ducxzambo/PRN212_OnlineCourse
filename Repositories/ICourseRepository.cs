using DataAccess.Models;

namespace Repositories;

public interface ICourseRepository
{
    Task<List<Course>> GetByInstructorAsync(int instructorId);
    Task<Course?> GetByIdWithDetailsAsync(int id);
    Task<List<Course>> GetAllWithDetailsAsync();

    /// <summary>All courses belonging to a category, with Instructor/Category/Enrollments loaded.</summary>
    Task<List<Course>> GetByCategoryAsync(int categoryId);

    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(int id);
}
