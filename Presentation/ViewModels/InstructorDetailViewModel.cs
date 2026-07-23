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
        Load();
    }

    private  void Load()
    {
        var courses = AppServices.CourseService.GetCoursesByInstructor(Instructor.Id);
        Courses.Clear();
        foreach (var course in courses) Courses.Add(course);
    }
}

