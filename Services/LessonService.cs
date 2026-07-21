using DataAccess.Models;
using Repositories;

namespace Services;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;

    public LessonService(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public Task<List<Lesson>> GetLessonsByCourseAsync(int courseId)
        => _lessonRepository.GetByCourseAsync(courseId);

    public async Task<(bool Success, string? Error)> CreateLessonAsync(Lesson lesson)
    {
        var error = Validate(lesson);
        if (error != null) return (false, error);

        lesson.OrderIndex = await _lessonRepository.GetNextOrderIndexAsync(lesson.CourseId);
        await _lessonRepository.AddAsync(lesson);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateLessonAsync(Lesson lesson)
    {
        var error = Validate(lesson);
        if (error != null) return (false, error);

        await _lessonRepository.UpdateAsync(lesson);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteLessonAsync(int lessonId)
    {
        await _lessonRepository.DeleteAsync(lessonId);
        return (true, null);
    }

    public async Task MoveLessonUpAsync(int lessonId, int courseId) => await MoveAsync(lessonId, courseId, -1);

    public async Task MoveLessonDownAsync(int lessonId, int courseId) => await MoveAsync(lessonId, courseId, 1);

    private async Task MoveAsync(int lessonId, int courseId, int direction)
    {
        var lessons = await _lessonRepository.GetByCourseAsync(courseId);
        var index = lessons.FindIndex(l => l.Id == lessonId);
        if (index < 0) return;

        var swapIndex = index + direction;
        if (swapIndex < 0 || swapIndex >= lessons.Count) return;

        await _lessonRepository.SwapOrderIndexAsync(
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
