using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;

namespace Presentation.ViewModels;

public class LessonEditViewModel : ViewModelBase
{
    private readonly int _courseId;
    private readonly int? _lessonId;
    private string _title = "";
    private string? _content;
    private string? _videoUrl;
    private int _durationMinutes;
    private string? _errorMessage;

    public bool IsEdit => _lessonId.HasValue;
    public string WindowTitle => IsEdit ? "Sửa bài học" : "Thêm bài học mới";

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string? Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }

    public string? VideoUrl
    {
        get => _videoUrl;
        set => SetProperty(ref _videoUrl, value);
    }

    public int DurationMinutes
    {
        get => _durationMinutes;
        set => SetProperty(ref _durationMinutes, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public ICommand SaveCommand { get; }

    public event EventHandler? SaveSucceeded;

    public LessonEditViewModel(int courseId, Lesson? lesson)
    {
        _courseId = courseId;

        if (lesson != null)
        {
            _lessonId = lesson.Id;
            Title = lesson.Title;
            Content = lesson.Content;
            VideoUrl = lesson.VideoUrl;
            DurationMinutes = lesson.DurationMinutes;
        }

        SaveCommand = new AsyncRelayCommand(SaveAsync);
    }

    private async Task SaveAsync()
    {
        ErrorMessage = null;

        var lesson = new Lesson
        {
            Id = _lessonId ?? 0,
            CourseId = _courseId,
            Title = Title?.Trim() ?? "",
            Content = string.IsNullOrWhiteSpace(Content) ? null : Content.Trim(),
            VideoUrl = string.IsNullOrWhiteSpace(VideoUrl) ? null : VideoUrl.Trim(),
            DurationMinutes = DurationMinutes
        };

        var (success, error) = IsEdit
            ? await AppServices.LessonService.UpdateLessonAsync(lesson)
            : await AppServices.LessonService.CreateLessonAsync(lesson);

        if (!success)
        {
            ErrorMessage = error;
            return;
        }

        SaveSucceeded?.Invoke(this, EventArgs.Empty);
    }
}
