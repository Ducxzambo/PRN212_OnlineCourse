using DataAccess.Models;

namespace Services;

public interface ICourseService
{
    Task<List<Course>> GetCoursesByInstructorAsync(int instructorId);
    Task<List<Category>> GetCategoriesAsync();

    /// <summary>All courses that belong to the given category (used by the Category screen).</summary>
    Task<List<Course>> GetCoursesByCategoryAsync(int categoryId);

    Task<(bool Success, string? Error)> CreateCourseAsync(Course course);
    Task<(bool Success, string? Error)> UpdateCourseAsync(Course course);
    Task<(bool Success, string? Error)> DeleteCourseAsync(int courseId);
}
