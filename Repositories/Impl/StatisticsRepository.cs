using Microsoft.EntityFrameworkCore;
using DataAccess.Models;
using Repositories.Models;

namespace Repositories.Impl;

public class StatisticsRepository : IStatisticsRepository
{
    public  DashboardStatistics GetDashboardStatistics()
    {
        using var context = new OnlineCourseManagementDbContext();

        var stats = new DashboardStatistics
        {
            TotalCategories = context.Categories.Count(),
            TotalInstructors = context.Instructors.Count(),
            TotalStudents = context.Students.Count(),
            TotalCourses = context.Courses.Count(),
            TotalLessons = context.Lessons.Count(),
            TotalEnrollments = context.Enrollments.Count(),
            RegisteredEnrollments = context.Enrollments.Count(e => e.Status == 0),
            InProgressEnrollments = context.Enrollments.Count(e => e.Status == 1),
            CompletedEnrollments = context.Enrollments.Count(e => e.Status == 2),
            CancelledEnrollments = context.Enrollments.Count(e => e.Status == 3),
        };

        stats.CoursesByCategory = context.Categories
            .Select(cat => new CategoryCourseCount
            {
                CategoryName = cat.Name,
                CourseCount = cat.Courses.Count
            })
            .OrderByDescending(x => x.CourseCount)
            .ToList();

        stats.TopCoursesByEnrollment = context.Courses
            .Include(c => c.Instructor)
            .Select(c => new TopCourseStat
            {
                CourseTitle = c.Title,
                InstructorName = c.Instructor.Account.FullName,
                EnrollmentCount = c.Enrollments.Count
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .Take(5)
            .ToList();

        return stats;
    }
}

