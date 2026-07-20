using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;
using Presentation.Views;

namespace Presentation.ViewModels;

public class LessonListViewModel : ViewModelBase
{
    private readonly Action _onBack;
    private Lesson? _selectedLesson;

    public Course Course { get; }
    public ObservableCollection<Lesson> Lessons { get; } = new();

    public Lesson? SelectedLesson
    {
        get => _selectedLesson;
        set => SetProperty(ref _selectedLesson, value);
    }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand MoveUpCommand { get; }
    public ICommand MoveDownCommand { get; }
    public ICommand BackCommand { get; }

    public LessonListViewModel(Course course, Action onBack)
    {
        Course = course;
        _onBack = onBack;

        AddCommand = new RelayCommand(Add);
        EditCommand = new RelayCommand(Edit, () => SelectedLesson != null);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedLesson != null);
        MoveUpCommand = new AsyncRelayCommand(MoveUpAsync, () => SelectedLesson != null);
        MoveDownCommand = new AsyncRelayCommand(MoveDownAsync, () => SelectedLesson != null);
        BackCommand = new RelayCommand(_onBack);
    }

    public async Task LoadAsync()
    {
        var previousSelectedId = SelectedLesson?.Id;

        var lessons = await AppServices.LessonService.GetLessonsByCourseAsync(Course.Id);
        Lessons.Clear();
        foreach (var lesson in lessons) Lessons.Add(lesson);

        if (previousSelectedId.HasValue)
            SelectedLesson = Lessons.FirstOrDefault(l => l.Id == previousSelectedId.Value);
    }

    private void Add()
    {
        var window = new LessonEditWindow(new LessonEditViewModel(Course.Id, null));
        if (window.ShowDialog() == true) _ = LoadAsync();
    }

    private void Edit()
    {
        if (SelectedLesson == null) return;

        var window = new LessonEditWindow(new LessonEditViewModel(Course.Id, SelectedLesson));
        if (window.ShowDialog() == true) _ = LoadAsync();
    }

    private async Task DeleteAsync()
    {
        if (SelectedLesson == null) return;

        var confirm = MessageBox.Show(
            $"Xóa bài học '{SelectedLesson.Title}'?", "Xác nhận xóa",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        await AppServices.LessonService.DeleteLessonAsync(SelectedLesson.Id);
        await LoadAsync();
    }

    private async Task MoveUpAsync()
    {
        if (SelectedLesson == null) return;
        var id = SelectedLesson.Id;
        await AppServices.LessonService.MoveLessonUpAsync(id, Course.Id);
        await LoadAsync();
        SelectedLesson = Lessons.FirstOrDefault(l => l.Id == id);
    }

    private async Task MoveDownAsync()
    {
        if (SelectedLesson == null) return;
        var id = SelectedLesson.Id;
        await AppServices.LessonService.MoveLessonDownAsync(id, Course.Id);
        await LoadAsync();
        SelectedLesson = Lessons.FirstOrDefault(l => l.Id == id);
    }
}
