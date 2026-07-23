using DataAccess.Models;

namespace Repositories;

public interface ICourseRepository
{
    List<Course> GetAll();
    List<Course> GetByInstructor(int instructorId);
    Course? GetByIdWithDetails(int id);
    List<Course> GetAllWithDetails();

    /// <summary>All courses belonging to a category, with Instructor/Category/Enrollments loaded.</summary>
    List<Course> GetByCategory(int categoryId);

    void Add(Course course);
    void Update(Course course);
    void Delete(int id);
}

