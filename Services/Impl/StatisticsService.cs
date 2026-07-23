using Repositories;
using Repositories.Models;

namespace Services.Impl;

public class StatisticsService : IStatisticsService
{
    private readonly IStatisticsRepository _statisticsRepository;

    public StatisticsService(IStatisticsRepository statisticsRepository)
    {
        _statisticsRepository = statisticsRepository;
    }

    public DashboardStatistics GetDashboardStatistics()
        => _statisticsRepository.GetDashboardStatistics();
}

