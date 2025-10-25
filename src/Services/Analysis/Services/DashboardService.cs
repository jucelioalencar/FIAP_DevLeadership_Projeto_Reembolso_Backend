using CorisSeguros.Analysis.Models;
using CorisSeguros.Analysis.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CorisSeguros.Analysis.Services;

public class DashboardService : IDashboardService
{
    private readonly AnalysisDbContext _context;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        AnalysisDbContext context,
        ILogger<DashboardService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DashboardMetrics> GetDashboardMetricsAsync()
    {
        try
        {
            var totalDocuments = await _context.AnalysisRecords.CountAsync();
            var approvedDocuments = await _context.AnalysisRecords.CountAsync(r => r.IsEligible);
            var rejectedDocuments = await _context.AnalysisRecords.CountAsync(r => !r.IsEligible);
            
            // Calcular TMA (Tempo Médio de Análise)
            var averageProcessingTime = await _context.AnalysisRecords
                .Where(r => r.AnalysisTimestamp >= DateTime.UtcNow.AddDays(-30))
                .Select(r => r.AnalysisTimestamp)
                .ToListAsync();

            var tma = averageProcessingTime.Any() 
                ? averageProcessingTime.Average(t => (DateTime.UtcNow - t).TotalHours)
                : 3.4; // Mock data

            // Calcular taxa de automação
            var automationRate = totalDocuments > 0 
                ? (double)approvedDocuments / totalDocuments * 100 
                : 52.0; // Mock data

            // Calcular taxa de erros
            var errorRate = totalDocuments > 0 
                ? (double)rejectedDocuments / totalDocuments * 100 
                : 3.2; // Mock data

            return new DashboardMetrics
            {
                TotalDocuments = totalDocuments,
                ApprovedDocuments = approvedDocuments,
                RejectedDocuments = rejectedDocuments,
                AverageProcessingTime = Math.Round(tma, 1),
                AutomationRate = Math.Round(automationRate, 1),
                ErrorRate = Math.Round(errorRate, 1),
                TmaReduction = 60.0, // Mock: redução de 60%
                ErrorReduction = 22.0 // Mock: redução de 22%
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao calcular métricas do dashboard");
            throw;
        }
    }

    public async Task<List<TmaEvolutionPoint>> GetTmaEvolutionAsync(int months)
    {
        try
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddMonths(-months);
            
            // Mock data para evolução do TMA
            var evolution = new List<TmaEvolutionPoint>();
            var currentDate = startDate;
            
            for (int i = 0; i < months; i++)
            {
                var monthName = currentDate.ToString("MMM", new System.Globalization.CultureInfo("pt-BR"));
                var tma = 9.0 - (i * 1.1); // Simulação de redução gradual
                
                evolution.Add(new TmaEvolutionPoint
                {
                    Month = monthName,
                    Tma = Math.Round(tma, 1)
                });
                
                currentDate = currentDate.AddMonths(1);
            }

            return evolution;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter evolução do TMA");
            throw;
        }
    }

    public async Task<StatusDistribution> GetStatusDistributionAsync()
    {
        try
        {
            var total = await _context.AnalysisRecords.CountAsync();
            var approved = await _context.AnalysisRecords.CountAsync(r => r.IsEligible);
            var rejected = await _context.AnalysisRecords.CountAsync(r => !r.IsEligible);
            
            // Mock data para status em análise e pendentes
            var inAnalysis = Math.Max(0, total / 10);
            var pending = Math.Max(0, total / 20);

            return new StatusDistribution
            {
                Total = total,
                Approved = approved,
                Rejected = rejected,
                InAnalysis = inAnalysis,
                Pending = pending
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter distribuição de status");
            throw;
        }
    }

    public async Task<List<DailyProcessing>> GetDailyProcessingAsync(int days)
    {
        try
        {
            var processing = new List<DailyProcessing>();
            var endDate = DateTime.UtcNow.Date;
            
            for (int i = days - 1; i >= 0; i--)
            {
                var date = endDate.AddDays(-i);
                var dayName = date.ToString("ddd", new System.Globalization.CultureInfo("pt-BR"));
                
                // Mock data para processamento diário
                var automatic = new Random().Next(10, 25);
                var manual = new Random().Next(8, 20);
                
                processing.Add(new DailyProcessing
                {
                    Day = dayName,
                    Automatic = automatic,
                    Manual = manual,
                    Total = automatic + manual
                });
            }

            return processing;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter processamento diário");
            throw;
        }
    }

    public async Task<List<ServiceStatus>> GetServiceStatusAsync()
    {
        try
        {
            // Mock data para status dos serviços
            var services = new List<ServiceStatus>
            {
                new ServiceStatus
                {
                    Name = "API Gateway",
                    Status = "Online",
                    Latency = 45,
                    LastCheck = DateTime.UtcNow
                },
                new ServiceStatus
                {
                    Name = "Serviço OCR",
                    Status = "Online",
                    Latency = 320,
                    LastCheck = DateTime.UtcNow
                },
                new ServiceStatus
                {
                    Name = "Serviço Validação",
                    Status = "Online",
                    Latency = 180,
                    LastCheck = DateTime.UtcNow
                },
                new ServiceStatus
                {
                    Name = "Serviço Análise",
                    Status = "Online",
                    Latency = 95,
                    LastCheck = DateTime.UtcNow
                }
            };

            return services;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter status dos serviços");
            throw;
        }
    }
}



