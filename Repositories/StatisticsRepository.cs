using Microsoft.EntityFrameworkCore;
using DataAccess.Models;
using Repositories.Models;

namespace Repositories;

public class StatisticsRepository : IStatisticsRepository
{
    public async Task<DashboardStatistics> GetDashboardStatisticsAsync()
    {
        using var context = new OnlineCourseManagementDbContext();

        var stats = new DashboardStatistics
        {
            TotalCategories = await context.Categories.CountAsync(),
            TotalInstructors = await context.Instructors.CountAsync(),
            TotalStudents = await context.Students.CountAsync(),
            TotalCourses = await context.Courses.CountAsync(),
            TotalLessons = await context.Lessons.CountAsync(),
            TotalEnrollments = await context.Enrollments.CountAsync(),
            RegisteredEnrollments = await context.Enrollments.CountAsync(e => e.Status == 0),
            InProgressEnrollments = await context.Enrollments.CountAsync(e => e.Status == 1),
            CompletedEnrollments = await context.Enrollments.CountAsync(e => e.Status == 2),
            CancelledEnrollments = await context.Enrollments.CountAsync(e => e.Status == 3),
        };

        stats.CoursesByCategory = await context.Categories
            .Select(cat => new CategoryCourseCount
            {
                CategoryName = cat.Name,
                CourseCount = cat.Courses.Count
            })
            .OrderByDescending(x => x.CourseCount)
            .ToListAsync();

        stats.TopCoursesByEnrollment = await context.Courses
            .Include(c => c.Instructor)
            .Select(c => new TopCourseStat
            {
                CourseTitle = c.Title,
                InstructorName = c.Instructor.FullName,
                EnrollmentCount = c.Enrollments.Count
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .Take(5)
            .ToListAsync();

        return stats;
    }
}
