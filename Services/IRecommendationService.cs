using Services.Models;

namespace Services;

public interface IRecommendationService
{
    /// <summary>
    /// Suggests courses for a student based on the categories of courses they have
    /// already (non-cancelled) enrolled in, excluding courses they are already in.
    /// Falls back to the most popular courses system-wide if the student has no
    /// usable history yet (cold start).
    /// </summary>
    Task<List<CourseRecommendation>> RecommendForStudentAsync(
        int studentId,
        int? instructorId,
        bool onlyInstructorCourses,
        int maxResults = 5);
}
