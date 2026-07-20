using System.Collections.ObjectModel;
using DataAccess.Models;
using Presentation.Helpers;

namespace Presentation.ViewModels;

public class StudentDetailViewModel : ViewModelBase
{
    public Student Student { get; }
    public ObservableCollection<Enrollment> History { get; } = new();

    public StudentDetailViewModel(Student student)
    {
        Student = student;
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        var history = await AppServices.StudentService.GetEnrollmentHistoryAsync(Student.Id);
        History.Clear();
        foreach (var enrollment in history) History.Add(enrollment);
    }
}
