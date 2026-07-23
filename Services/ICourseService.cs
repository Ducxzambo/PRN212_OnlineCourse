using DataAccess.Models;

namespace Services;

public interface ICourseService
{
    List<Course> GetAllCourses();
    List<Course> GetCoursesByInstructor(int instructorId);
    List<Category> GetCategories();

    /// <summary>All courses that belong to the given category (used by the Category screen).</summary>
    List<Course> GetCoursesByCategory(int categoryId);

    (bool Success, string? Error) CreateCourse(Course course);
    (bool Success, string? Error) UpdateCourse(Course course);
    (bool Success, string? Error) DeleteCourse(int courseId);
}

