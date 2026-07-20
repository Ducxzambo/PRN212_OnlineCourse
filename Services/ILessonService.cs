using DataAccess.Models;

namespace Services;

public interface ILessonService
{
    Task<List<Lesson>> GetLessonsByCourseAsync(int courseId);
    Task<(bool Success, string? Error)> CreateLessonAsync(Lesson lesson);
    Task<(bool Success, string? Error)> UpdateLessonAsync(Lesson lesson);
    Task<(bool Success, string? Error)> DeleteLessonAsync(int lessonId);
    Task MoveLessonUpAsync(int lessonId, int courseId);
    Task MoveLessonDownAsync(int lessonId, int courseId);
}
