using Repositories;
using Repositories.Models;

namespace Services;

public class StatisticsService : IStatisticsService
{
    private readonly IStatisticsRepository _statisticsRepository;

    public StatisticsService(IStatisticsRepository statisticsRepository)
    {
        _statisticsRepository = statisticsRepository;
    }

    public Task<DashboardStatistics> GetDashboardStatisticsAsync()
        => _statisticsRepository.GetDashboardStatisticsAsync();
}
