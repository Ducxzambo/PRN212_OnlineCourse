using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Impl;

public class LessonRepository : ILessonRepository
{
    public  List<Lesson> GetByCourse(int courseId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Lessons
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.OrderIndex)
            .AsNoTracking()
            .ToList();
    }

    public  Lesson? GetById(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Lessons.AsNoTracking().FirstOrDefault(l => l.Id == id);
    }

    public  int GetNextOrderIndex(int courseId)
    {
        using var context = new OnlineCourseManagementDbContext();
        var maxOrder = context.Lessons
            .Where(l => l.CourseId == courseId)
            .Select(l => (int?)l.OrderIndex)
            .Max();
        return (maxOrder ?? 0) + 1;
    }

    public  void Add(Lesson lesson)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Lessons.Add(lesson);
        context.SaveChanges();
    }

    public  void Update(Lesson lesson)
    {
        using var context = new OnlineCourseManagementDbContext();
        var existing = context.Lessons.Find(lesson.Id);
        if (existing == null) return;

        existing.Title = lesson.Title;
        existing.Content = lesson.Content;
        existing.VideoUrl = lesson.VideoUrl;
        existing.DurationMinutes = lesson.DurationMinutes;

        context.SaveChanges();
    }

    public  void Delete(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        var lesson = context.Lessons.Find(id);
        if (lesson != null)
        {
            context.Lessons.Remove(lesson);
            context.SaveChanges();
        }
    }

    public  void SwapOrderIndex(int lessonId1, int orderIndex1, int lessonId2, int orderIndex2)
    {
        using var context = new OnlineCourseManagementDbContext();
        var l1 = context.Lessons.Find(lessonId1);
        var l2 = context.Lessons.Find(lessonId2);
        if (l1 != null) l1.OrderIndex = orderIndex1;
        if (l2 != null) l2.OrderIndex = orderIndex2;
        context.SaveChanges();
    }
}

