using Microsoft.AspNetCore.Mvc;
using CorisSeguros.Analysis.Services;
using CorisSeguros.Analysis.Models;
using CorisSeguros.Analysis.DTOs;

namespace CorisSeguros.Analysis.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IDashboardService dashboardService,
        ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet("metrics")]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        try
        {
            var metrics = await _dashboardService.GetDashboardMetricsAsync();
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter métricas do dashboard");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("tma-evolution")]
    public async Task<IActionResult> GetTmaEvolution([FromQuery] int months = 5)
    {
        try
        {
            var evolution = await _dashboardService.GetTmaEvolutionAsync(months);
            return Ok(evolution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter evolução do TMA");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("status-distribution")]
    public async Task<IActionResult> GetStatusDistribution()
    {
        try
        {
            var distribution = await _dashboardService.GetStatusDistributionAsync();
            return Ok(distribution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter distribuição de status");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("daily-processing")]
    public async Task<IActionResult> GetDailyProcessing([FromQuery] int days = 7)
    {
        try
        {
            var processing = await _dashboardService.GetDailyProcessingAsync(days);
            return Ok(processing);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter processamento diário");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("service-status")]
    public async Task<IActionResult> GetServiceStatus()
    {
        try
        {
            var status = await _dashboardService.GetServiceStatusAsync();
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter status dos serviços");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}



