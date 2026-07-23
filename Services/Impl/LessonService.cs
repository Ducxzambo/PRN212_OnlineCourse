using DataAccess.Models;
using Repositories;

namespace Services.Impl;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;

    public LessonService(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public List<Lesson> GetLessonsByCourse(int courseId)
        => _lessonRepository.GetByCourse(courseId);

    public  (bool Success, string? Error) CreateLesson(Lesson lesson)
    {
        var error = Validate(lesson);
        if (error != null) return (false, error);

        lesson.OrderIndex = _lessonRepository.GetNextOrderIndex(lesson.CourseId);
        _lessonRepository.Add(lesson);
        return (true, null);
    }

    public  (bool Success, string? Error) UpdateLesson(Lesson lesson)
    {
        var error = Validate(lesson);
        if (error != null) return (false, error);

        _lessonRepository.Update(lesson);
        return (true, null);
    }

    public  (bool Success, string? Error) DeleteLesson(int lessonId)
    {
        _lessonRepository.Delete(lessonId);
        return (true, null);
    }

    public  void MoveLessonUp(int lessonId, int courseId) => Move(lessonId, courseId, -1);

    public  void MoveLessonDown(int lessonId, int courseId) => Move(lessonId, courseId, 1);

    private  void Move(int lessonId, int courseId, int direction)
    {
        var lessons = _lessonRepository.GetByCourse(courseId);
        var index = lessons.FindIndex(l => l.Id == lessonId);
        if (index < 0) return;

        var swapIndex = index + direction;
        if (swapIndex < 0 || swapIndex >= lessons.Count) return;

        _lessonRepository.SwapOrderIndex(
            lessons[index].Id, lessons[swapIndex].OrderIndex,
            lessons[swapIndex].Id, lessons[index].OrderIndex);
    }

    private static string? Validate(Lesson lesson)
    {
        if (string.IsNullOrWhiteSpace(lesson.Title))
            return "Tên bài học không được để trống.";
        if (lesson.Title.Length > 200)
            return "Tên bài học không được vượt quá 200 ký tự.";
        if (lesson.DurationMinutes <= 0)
            return "Thời lượng bài học phải lớn hơn 0.";
        if (lesson.VideoUrl != null && lesson.VideoUrl.Length > 500)
            return "Link video không được vượt quá 500 ký tự.";

        return null;
    }
}

