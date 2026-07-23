using DataAccess.Models;

namespace Presentation.ViewModels;

public class LessonProgressItem
{
    public Lesson Lesson { get; init; } = null!;
    public bool IsCompleted { get; init; }
    public string CompletionStatus => IsCompleted ? "Đã hoàn thành" : "Chưa hoàn thành";
}
