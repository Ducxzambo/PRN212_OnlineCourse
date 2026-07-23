using Repositories.Models;

namespace Repositories;

public interface IStatisticsRepository
{
    DashboardStatistics GetDashboardStatistics();
}

