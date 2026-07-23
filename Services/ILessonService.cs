using DataAccess.Models;

namespace Services;

public interface ILessonService
{
    List<Lesson> GetLessonsByCourse(int courseId);
    (bool Success, string? Error) CreateLesson(Lesson lesson);
    (bool Success, string? Error) UpdateLesson(Lesson lesson);
    (bool Success, string? Error) DeleteLesson(int lessonId);
    void MoveLessonUp(int lessonId, int courseId);
    void MoveLessonDown(int lessonId, int courseId);
}

