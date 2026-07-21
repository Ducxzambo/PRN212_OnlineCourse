using Repositories.Models;

namespace Repositories;

public interface IStatisticsRepository
{
    Task<DashboardStatistics> GetDashboardStatisticsAsync();
}
