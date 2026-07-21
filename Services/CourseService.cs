using DataAccess.Models;
using Repositories;

namespace Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CourseService(ICourseRepository courseRepository, ICategoryRepository categoryRepository)
    {
        _courseRepository = courseRepository;
        _categoryRepository = categoryRepository;
    }

    public Task<List<Course>> GetCoursesByInstructorAsync(int instructorId)
        => _courseRepository.GetByInstructorAsync(instructorId);

    public Task<List<Category>> GetCategoriesAsync()
        => _categoryRepository.GetAllAsync();

    public async Task<(bool Success, string? Error)> CreateCourseAsync(Course course)
    {
        var error = Validate(course);
        if (error != null) return (false, error);

        course.CreatedDate = DateTime.Now;
        await _courseRepository.AddAsync(course);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateCourseAsync(Course course)
    {
        var error = Validate(course);
        if (error != null) return (false, error);

        await _courseRepository.UpdateAsync(course);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteCourseAsync(int courseId)
    {
        var course = await _courseRepository.GetByIdWithDetailsAsync(courseId);
        if (course == null) return (false, "Không tìm thấy khóa học.");

        if (course.Enrollments.Count > 0)
            return (false, "Không thể xóa khóa học đã có học viên đăng ký. Hãy xử lý học viên trước.");

        await _courseRepository.DeleteAsync(courseId);
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
