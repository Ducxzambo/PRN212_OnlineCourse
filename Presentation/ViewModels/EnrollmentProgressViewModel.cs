using System.Collections.ObjectModel;
using DataAccess.Models;
using Presentation.Helpers;

namespace Presentation.ViewModels;

/// <summary>Read-only lesson progress for one enrollment, used by instructors.</summary>
public class EnrollmentProgressViewModel : ViewModelBase
{
    public Enrollment Enrollment { get; }
    public Student Student => Enrollment.Student;
    public Course Course => Enrollment.Course;
    public ObservableCollection<LessonProgressItem> Lessons { get; } = new();

    public EnrollmentProgressViewModel(Enrollment enrollment)
    {
        Enrollment = enrollment;
        Load();
    }

    private void Load()
    {
        var completedLessonIds = AppServices.StudentService.GetCompletedLessonIds(Enrollment.Id).ToHashSet();
        foreach (var lesson in AppServices.LessonService.GetLessonsByCourse(Enrollment.CourseId))
            Lessons.Add(new LessonProgressItem { Lesson = lesson, IsCompleted = completedLessonIds.Contains(lesson.Id) });
    }
}
