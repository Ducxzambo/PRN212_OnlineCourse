using System.Collections.ObjectModel;
using DataAccess.Models;
using Presentation.Helpers;

namespace Presentation.ViewModels;

/// <summary>Read-only course enrollment history for a student.</summary>
public class StudentDetailViewModel : ViewModelBase
{
    public Student Student { get; }
    public ObservableCollection<Enrollment> History { get; } = new();

    public StudentDetailViewModel(Student student)
    {
        Student = student;
        Load();
    }

    private void Load()
    {
        foreach (var enrollment in AppServices.StudentService.GetEnrollmentHistory(Student.Id))
            History.Add(enrollment);
    }
}
