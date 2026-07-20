using DataAccess.Models;

namespace Services;

public interface ICourseService
{
    Task<List<Course>> GetCoursesByInstructorAsync(int instructorId);
    Task<List<Category>> GetCategoriesAsync();
    Task<(bool Success, string? Error)> CreateCourseAsync(Course course);
    Task<(bool Success, string? Error)> UpdateCourseAsync(Course course);
    Task<(bool Success, string? Error)> DeleteCourseAsync(int courseId);
}
