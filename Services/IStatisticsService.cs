using Repositories.Models;

namespace Services;

public interface IStatisticsService
{
    DashboardStatistics GetDashboardStatistics();
}

