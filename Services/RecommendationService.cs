using Repositories;
using Services.Models;

namespace Services;

public class RecommendationService : IRecommendationService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public RecommendationService(ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<List<CourseRecommendation>> RecommendForStudentAsync(
        int studentId,
        int? instructorId,
        bool onlyInstructorCourses,
        int maxResults = 5)
    {
        var history = await _enrollmentRepository.GetByStudentAsync(studentId);

        // "Studied" = every enrollment that wasn't cancelled (Registered/InProgress/Completed all count as interest).
        var activeHistory = history.Where(e => e.Status != (int)EnrollmentStatus.Cancelled).ToList();

        var enrolledCourseIds = activeHistory.Select(e => e.CourseId).ToHashSet();
        var studiedCategoryIds = activeHistory.Select(e => e.Course.CategoryId).Distinct().ToList();

        var enrollmentCounts = await _enrollmentRepository.GetEnrollmentCountsByCourseAsync();

        var allCourses = await _courseRepository.GetAllWithDetailsAsync();
        var candidates = allCourses.Where(c => !enrolledCourseIds.Contains(c.Id));

        if (onlyInstructorCourses && instructorId.HasValue)
            candidates = candidates.Where(c => c.InstructorId == instructorId.Value);

        var candidateList = candidates.ToList();

        var categoryMatches = studiedCategoryIds.Count > 0
            ? candidateList.Where(c => studiedCategoryIds.Contains(c.CategoryId)).ToList()
            : new List<DataAccess.Models.Course>();

        var isFallback = categoryMatches.Count == 0;
        var pool = isFallback ? candidateList : categoryMatches;

        var ranked = pool
            .OrderByDescending(c => enrollmentCounts.TryGetValue(c.Id, out var count) ? count : 0)
            .ThenByDescending(c => c.CreatedDate)
            .Take(maxResults)
            .ToList();

        return ranked.Select(c => new CourseRecommendation
        {
            Course = c,
            EnrollmentCount = enrollmentCounts.TryGetValue(c.Id, out var cnt) ? cnt : 0,
            Reason = isFallback
                ? "Học viên chưa có lịch sử phù hợp - đây là khóa học phổ biến nhất hệ thống"
                : $"Cùng danh mục \"{c.Category.Name}\" với khóa học học viên đã tham gia"
        }).ToList();
    }
}
