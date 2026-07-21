using Repositories;
using Services;

namespace Presentation.Helpers;

/// <summary>
/// Simple hand-rolled composition root. Every repository opens its own short-lived
/// DbContext per call, so these instances are stateless and safe to share app-wide.
/// </summary>
public static class AppServices
{
    public static IInstructorService InstructorService { get; } =
        new InstructorService(new InstructorRepository());

    public static ICourseService CourseService { get; } =
        new CourseService(new CourseRepository(), new CategoryRepository());

    public static ILessonService LessonService { get; } =
        new LessonService(new LessonRepository());

    public static IStudentService StudentService { get; } =
        new StudentService(new EnrollmentRepository(), new StudentRepository());

    public static IRecommendationService RecommendationService { get; } =
        new RecommendationService(new CourseRepository(), new EnrollmentRepository());

    public static IAccountService AccountService { get; } =
        new AccountService(new AccountRepository(), new InstructorRepository(), new StudentRepository());

    public static ICategoryService CategoryService { get; } =
        new CategoryService(new CategoryRepository());

    public static IStatisticsService StatisticsService { get; } =
        new StatisticsService(new StatisticsRepository());
}
