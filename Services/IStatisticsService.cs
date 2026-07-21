using Repositories.Models;

namespace Services;

public interface IStatisticsService
{
    Task<DashboardStatistics> GetDashboardStatisticsAsync();
}
