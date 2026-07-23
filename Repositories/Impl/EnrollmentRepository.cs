using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Impl;

public class EnrollmentRepository : IEnrollmentRepository
{
    public  List<Enrollment> GetByInstructor(int instructorId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Enrollments
            .Include(e => e.Student).ThenInclude(s => s.Account)
            .Include(e => e.Student).ThenInclude(s => s.Account)
            .Include(e => e.Course)
            .Where(e => e.Course.InstructorId == instructorId)
            .OrderByDescending(e => e.EnrollDate)
            .AsNoTracking()
            .ToList();
    }

    public  List<Enrollment> GetByStudent(int studentId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Enrollments
            .Include(e => e.Course)
                .ThenInclude(c => c.Category)
            .Include(e => e.Course)
                .ThenInclude(c => c.Instructor)
            .Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.EnrollDate)
            .AsNoTracking()
            .ToList();
    }

    public  Enrollment? GetById(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Enrollments
            .Include(e => e.Student).ThenInclude(s => s.Account)
            .Include(e => e.Course)
            .AsNoTracking()
            .FirstOrDefault(e => e.Id == id);
    }

    public  bool Exists(int studentId, int courseId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId);
    }

    public  void Add(Enrollment enrollment)
    {
        using var context = new OnlineCourseManagementDbContext();
        context.Enrollments.Add(enrollment);
        context.SaveChanges();
    }

    public  bool UpdateStatus(int enrollmentId, int newStatus)
    {
        using var context = new OnlineCourseManagementDbContext();
        var enrollment = context.Enrollments.Find(enrollmentId);
        if (enrollment == null) return false;

        enrollment.Status = newStatus;
        context.SaveChanges();
        return true;
    }

    public bool CompleteLesson(int enrollmentId, int lessonId)
        => SetLessonCompletion(enrollmentId, lessonId, true);

    public bool SetLessonCompletion(int enrollmentId, int lessonId, bool isCompleted)
    {
        using var context = new OnlineCourseManagementDbContext();
        var enrollment = context.Enrollments.FirstOrDefault(e => e.Id == enrollmentId);
        if (enrollment == null || !context.Lessons.Any(l => l.Id == lessonId && l.CourseId == enrollment.CourseId)) return false;

        var completion = context.LessonCompletions.FirstOrDefault(x => x.EnrollmentId == enrollmentId && x.LessonId == lessonId);
        if (isCompleted && completion == null)
            context.LessonCompletions.Add(new LessonCompletion { EnrollmentId = enrollmentId, LessonId = lessonId, CompletedDate = DateTime.Now });
        else if (!isCompleted && completion != null)
            context.LessonCompletions.Remove(completion);

        context.SaveChanges();

        var total = context.Lessons.Count(l => l.CourseId == enrollment.CourseId);
        var completed = context.LessonCompletions.Count(x => x.EnrollmentId == enrollmentId);
        enrollment.Progress = total == 0 ? 0 : Math.Round(completed * 100m / total, 2);
        enrollment.Status = enrollment.Progress >= 100 ? 2 : 1;
        context.SaveChanges();
        return true;
    }

    public  List<int> GetCompletedLessonIds(int enrollmentId)
    {
        using var context = new OnlineCourseManagementDbContext();
        return context.LessonCompletions.Where(x => x.EnrollmentId == enrollmentId).Select(x => x.LessonId).ToList();
    }

    public  void Delete(int id)
    {
        using var context = new OnlineCourseManagementDbContext();
        var enrollment = context.Enrollments.Find(id);
        if (enrollment != null)
        {
            context.Enrollments.Remove(enrollment);
            context.SaveChanges();
        }
    }

    public  Dictionary<int, int> GetEnrollmentCountsByCourse()
    {
        using var context = new OnlineCourseManagementDbContext();
        var grouped = context.Enrollments
            .GroupBy(e => e.CourseId)
            .Select(g => new { CourseId = g.Key, Count = g.Count() })
            .ToList();

        return grouped.ToDictionary(x => x.CourseId, x => x.Count);
    }
}

