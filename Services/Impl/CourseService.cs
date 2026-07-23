using DataAccess.Models;
using Repositories;

namespace Services.Impl;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CourseService(ICourseRepository courseRepository, ICategoryRepository categoryRepository)
    {
        _courseRepository = courseRepository;
        _categoryRepository = categoryRepository;
    }

    public List<Course> GetCoursesByInstructor(int instructorId)
        => _courseRepository.GetByInstructor(instructorId);

    public List<Course> GetAllCourses() => _courseRepository.GetAll();

    public List<Category> GetCategories()
        => _categoryRepository.GetAll();

    public List<Course> GetCoursesByCategory(int categoryId)
        => _courseRepository.GetByCategory(categoryId);

    public  (bool Success, string? Error) CreateCourse(Course course)
    {
        var error = Validate(course);
        if (error != null) return (false, error);

        course.CreatedDate = DateTime.Now;
        _courseRepository.Add(course);
        return (true, null);
    }

    public  (bool Success, string? Error) UpdateCourse(Course course)
    {
        var error = Validate(course);
        if (error != null) return (false, error);

        _courseRepository.Update(course);
        return (true, null);
    }

    public  (bool Success, string? Error) DeleteCourse(int courseId)
    {
        var course = _courseRepository.GetByIdWithDetails(courseId);
        if (course == null) return (false, "Không tìm thấy khóa học.");

        if (course.Enrollments.Count > 0)
            return (false, "Không thể xóa khóa học đã có học viên đăng ký. Hãy xử lý học viên trước.");

        _courseRepository.Delete(courseId);
        return (true, null);
    }

    private static string? Validate(Course course)
    {
        if (string.IsNullOrWhiteSpace(course.Title))
            return "Tên khóa học không được để trống.";
        if (course.Title.Length > 200)
            return "Tên khóa học không được vượt quá 200 ký tự.";
        if (course.Description != null && course.Description.Length > 1000)
            return "Mô tả không được vượt quá 1000 ký tự.";
        if (course.Price < 0)
            return "Giá khóa học không được âm.";
        if (course.DurationHours <= 0)
            return "Thời lượng khóa học phải lớn hơn 0.";
        if (course.CategoryId <= 0)
            return "Vui lòng chọn danh mục.";

        return null;
    }
}

