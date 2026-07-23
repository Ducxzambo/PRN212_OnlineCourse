using System.Collections.ObjectModel;
using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;

namespace Presentation.ViewModels;

/// <summary>Read-only lesson browser for the course selected by an administrator.</summary>
public class AdminLessonListViewModel : ViewModelBase
{
    private readonly Action _onBack;

    public Course Course { get; }
    public ObservableCollection<Lesson> Lessons { get; } = new();
    public ICommand BackCommand { get; }
    public ICommand RefreshCommand { get; }

    public AdminLessonListViewModel(Course course, Action onBack)
    {
        Course = course;
        _onBack = onBack;
        BackCommand = new RelayCommand(_onBack);
        RefreshCommand = new RelayCommand(Load);
    }

    public void Load()
    {
        Lessons.Clear();
        foreach (var lesson in AppServices.LessonService.GetLessonsByCourse(Course.Id))
            Lessons.Add(lesson);
    }
}
