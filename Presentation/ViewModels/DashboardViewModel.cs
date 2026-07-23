using System.Collections.ObjectModel;
using System.Windows.Input;
using Presentation.Helpers;
using Repositories.Models;

namespace Presentation.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private DashboardStatistics _stats = new();

    public DashboardStatistics Stats
    {
        get => _stats;
        set => SetProperty(ref _stats, value);
    }

    public ObservableCollection<CategoryCourseCount> CoursesByCategory { get; } = new();
    public ObservableCollection<TopCourseStat> TopCourses { get; } = new();

    public ICommand RefreshCommand { get; }

    public DashboardViewModel()
    {
        RefreshCommand = new RelayCommand(Load);
    }

    public  void Load()
    {
        Stats = AppServices.StatisticsService.GetDashboardStatistics();

        CoursesByCategory.Clear();
        foreach (var item in Stats.CoursesByCategory)
        {
            CoursesByCategory.Add(item);
        }

        TopCourses.Clear();
        foreach (var item in Stats.TopCoursesByEnrollment)
        {
            TopCourses.Add(item);
        }
    }
}

