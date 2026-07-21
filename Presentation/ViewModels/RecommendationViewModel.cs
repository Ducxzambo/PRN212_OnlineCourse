using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Presentation.Helpers;
using Services.Models;

namespace Presentation.ViewModels;

public class RecommendationViewModel : ViewModelBase
{
    private Student? _selectedStudent;
    private bool _onlyMyCourses;
    private string? _infoMessage;

    public ObservableCollection<Student> Students { get; } = new();
    public ObservableCollection<CourseRecommendation> Recommendations { get; } = new();

    public Student? SelectedStudent
    {
        get => _selectedStudent;
        set => SetProperty(ref _selectedStudent, value);
    }

    public bool OnlyMyCourses
    {
        get => _onlyMyCourses;
        set => SetProperty(ref _onlyMyCourses, value);
    }

    public string? InfoMessage
    {
        get => _infoMessage;
        set => SetProperty(ref _infoMessage, value);
    }

    public ICommand RecommendCommand { get; }
    public ICommand EnrollFromRecommendationCommand { get; }

    public RecommendationViewModel()
    {
        RecommendCommand = new AsyncRelayCommand(RecommendAsync, () => SelectedStudent != null);
        EnrollFromRecommendationCommand = new AsyncRelayCommand(EnrollFromParameterAsync);
    }

    public async Task LoadAsync()
    {
        if (InstructorSession.Current == null) return;

        var previousSelectedId = SelectedStudent?.Id;

        var roster = await AppServices.StudentService.GetRosterByInstructorAsync(InstructorSession.Current.Id);
        var distinctStudents = roster
            .Select(e => e.Student)
            .GroupBy(s => s.Id)
            .Select(g => g.First())
            .OrderBy(s => s.FullName)
            .ToList();

        Students.Clear();
        foreach (var student in distinctStudents) Students.Add(student);

        SelectedStudent = Students.FirstOrDefault(s => s.Id == previousSelectedId) ?? Students.FirstOrDefault();
    }

    private async Task RecommendAsync()
    {
        if (SelectedStudent == null) return;

        Recommendations.Clear();
        InfoMessage = null;

        var instructorId = InstructorSession.Current?.Id;
        var results = await AppServices.RecommendationService.RecommendForStudentAsync(
            SelectedStudent.Id, instructorId, OnlyMyCourses);

        if (results.Count == 0)
        {
            InfoMessage = "Không tìm thấy khóa học phù hợp để đề xuất.";
            return;
        }

        foreach (var recommendation in results) Recommendations.Add(recommendation);
    }

    private async Task EnrollFromParameterAsync(object? parameter)
    {
        if (parameter is CourseRecommendation recommendation)
            await EnrollAsync(recommendation);
    }

    private async Task EnrollAsync(CourseRecommendation recommendation)
    {
        if (SelectedStudent == null) return;

        var (success, error) = await AppServices.StudentService.EnrollExistingStudentAsync(SelectedStudent.Id, recommendation.Course.Id);
        if (!success)
        {
            MessageBox.Show(error, "Không thể ghi danh", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        MessageBox.Show("Đã ghi danh học viên vào khóa học.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        await RecommendAsync();
    }
}
