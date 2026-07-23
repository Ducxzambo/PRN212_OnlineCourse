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
        DeleteCommand = new RelayCommand(Delete, () => SelectedLesson != null);
        MoveUpCommand = new RelayCommand(MoveUp, () => SelectedLesson != null);
        MoveDownCommand = new RelayCommand(MoveDown, () => SelectedLesson != null);
        BackCommand = new RelayCommand(_onBack);
    }

    public  void Load()
    {
        var previousSelectedId = SelectedLesson?.Id;

        var lessons = AppServices.LessonService.GetLessonsByCourse(Course.Id);
        Lessons.Clear();
        foreach (var lesson in lessons) Lessons.Add(lesson);

        if (previousSelectedId.HasValue)
            SelectedLesson = Lessons.FirstOrDefault(l => l.Id == previousSelectedId.Value);
    }

    private void Add()
    {
        var window = new LessonEditWindow(new LessonEditViewModel(Course.Id, null));
        if (window.ShowDialog() == true) Load();
    }

    private void Edit()
    {
        if (SelectedLesson == null) return;

        var window = new LessonEditWindow(new LessonEditViewModel(Course.Id, SelectedLesson));
        if (window.ShowDialog() == true) Load();
    }

    private  void Delete()
    {
        if (SelectedLesson == null) return;

        var confirm = MessageBox.Show(
            $"Xóa bài học '{SelectedLesson.Title}'?", "Xác nhận xóa",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        AppServices.LessonService.DeleteLesson(SelectedLesson.Id);
        Load();
    }

    private  void MoveUp()
    {
        if (SelectedLesson == null) return;
        var id = SelectedLesson.Id;
        AppServices.LessonService.MoveLessonUp(id, Course.Id);
        Load();
        SelectedLesson = Lessons.FirstOrDefault(l => l.Id == id);
    }

    private  void MoveDown()
    {
        if (SelectedLesson == null) return;
        var id = SelectedLesson.Id;
        AppServices.LessonService.MoveLessonDown(id, Course.Id);
        Load();
        SelectedLesson = Lessons.FirstOrDefault(l => l.Id == id);
    }
}

