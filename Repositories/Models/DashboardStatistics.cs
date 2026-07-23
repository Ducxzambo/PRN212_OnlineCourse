namespace Repositories.Models;

public class DashboardStatistics
{
    public int TotalCategories { get; set; }
    public int TotalInstructors { get; set; }
    public int TotalStudents { get; set; }
    public int TotalCourses { get; set; }
    public int TotalLessons { get; set; }
    public int TotalEnrollments { get; set; }

    public int RegisteredEnrollments { get; set; }
    public int InProgressEnrollments { get; set; }
    public int CompletedEnrollments { get; set; }
    public int CancelledEnrollments { get; set; }

    public List<CategoryCourseCount> CoursesByCategory { get; set; } = new();
    public List<TopCourseStat> TopCoursesByEnrollment { get; set; } = new();
}

public class CategoryCourseCount
{
    public string CategoryName { get; set; } = "";
    public int CourseCount { get; set; }
}

public class TopCourseStat
{
    public string CourseTitle { get; set; } = "";
    public string InstructorName { get; set; } = "";
    public int EnrollmentCount { get; set; }
}

