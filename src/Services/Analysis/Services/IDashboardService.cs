using CorisSeguros.Analysis.Models;

namespace CorisSeguros.Analysis.Services;

public interface IDashboardService
{
    Task<DashboardMetrics> GetDashboardMetricsAsync();
    Task<List<TmaEvolutionPoint>> GetTmaEvolutionAsync(int months);
    Task<StatusDistribution> GetStatusDistributionAsync();
    Task<List<DailyProcessing>> GetDailyProcessingAsync(int days);
    Task<List<ServiceStatus>> GetServiceStatusAsync();
}



