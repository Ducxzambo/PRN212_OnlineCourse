using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class LessonRepository : ILessonRepository
{
    public async Task<List<Lesson>> GetByCourseAsync(int courseId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Lessons
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.OrderIndex)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Lesson?> GetByIdAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return await context.Lessons.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<int> GetNextOrderIndexAsync(int courseId)
    {
        using var context = new OnlineCourseManagementDbContext();
        var maxOrder = await context.Lessons
            .Where(l => l.CourseId == courseId)
            .Select(l => (int?)l.OrderIndex)
            .MaxAsync();
        return (maxOrder ?? 0) + 1;
    }

    public async Task AddAsync(Lesson lesson)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Lessons.Add(lesson);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Lesson lesson)
    {
        using var context = new OnlineCourseManagementDbContext();
        var existing = await context.Lessons.FindAsync(lesson.Id);
        if (existing == null) return;

        existing.Title = lesson.Title;
        existing.Content = lesson.Content;
        existing.VideoUrl = lesson.VideoUrl;
        existing.DurationMinutes = lesson.DurationMinutes;

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        var lesson = await context.Lessons.FindAsync(id);
        if (lesson != null)
        {
            context.Lessons.Remove(lesson);
            await context.SaveChangesAsync();
        }
    }

    public async Task SwapOrderIndexAsync(int lessonId1, int orderIndex1, int lessonId2, int orderIndex2)
    {
        using var context = new OnlineCourseManagementDbContext();
        var l1 = await context.Lessons.FindAsync(lessonId1);
        var l2 = await context.Lessons.FindAsync(lessonId2);
        if (l1 != null) l1.OrderIndex = orderIndex1;
        if (l2 != null) l2.OrderIndex = orderIndex2;
        await context.SaveChangesAsync();
    }
}
