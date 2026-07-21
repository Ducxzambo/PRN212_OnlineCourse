using System.Collections.ObjectModel;
using DataAccess.Models;
using Presentation.Helpers;

namespace Presentation.ViewModels;

public class InstructorDetailViewModel : ViewModelBase
{
    public Instructor Instructor { get; }
    public ObservableCollection<Course> Courses { get; } = new();

    public InstructorDetailViewModel(Instructor instructor)
    {
        Instructor = instructor;
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        var courses = await AppServices.CourseService.GetCoursesByInstructorAsync(Instructor.Id);
        Courses.Clear();
        foreach (var course in courses) Courses.Add(course);
    }
}
