using Microsoft.AspNetCore.Mvc;
using CorisSeguros.Analysis.Services;
using CorisSeguros.Analysis.Models;
using CorisSeguros.Analysis.DTOs;

namespace CorisSeguros.Analysis.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalysisController : ControllerBase
{
    private readonly IAnalysisService _analysisService;
    private readonly ILogger<AnalysisController> _logger;

    public AnalysisController(
        IAnalysisService analysisService,
        ILogger<AnalysisController> logger)
    {
        _analysisService = analysisService;
        _logger = logger;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeReimbursement([FromBody] AnalysisRequestDto request)
    {
        try
        {
            _logger.LogInformation("Iniciando análise de reembolso para documento {DocumentId}", request.DocumentId);

            var analysisResult = await _analysisService.AnalyzeReimbursementAsync(
                request.DocumentId,
                request.FlightData,
                request.ValidationResult);

            _logger.LogInformation("Análise concluída para documento {DocumentId}. Elegível: {IsEligible}", 
                request.DocumentId, analysisResult.IsEligible);

            return Ok(new AnalysisResponseDto
            {
                DocumentId = analysisResult.DocumentId,
                IsEligible = analysisResult.IsEligible,
                Recommendation = analysisResult.Recommendation,
                Confidence = analysisResult.Confidence,
                Reasoning = analysisResult.Reasoning,
                BusinessRulesApplied = analysisResult.BusinessRulesApplied,
                AnalysisTimestamp = analysisResult.AnalysisTimestamp
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante análise de reembolso para documento {DocumentId}", request.DocumentId);
            return StatusCode(500, "Erro interno do servidor durante análise");
        }
    }

    [HttpGet("rules")]
    public async Task<IActionResult> GetBusinessRules()
    {
        try
        {
            var rules = await _analysisService.GetBusinessRulesAsync();
            return Ok(rules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar regras de negócio");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("rules")]
    public async Task<IActionResult> CreateBusinessRule([FromBody] BusinessRuleDto ruleDto)
    {
        try
        {
            var rule = new BusinessRule
            {
                Id = Guid.NewGuid(),
                Name = ruleDto.Name,
                Description = ruleDto.Description,
                Condition = ruleDto.Condition,
                Action = ruleDto.Action,
                Priority = ruleDto.Priority,
                IsActive = ruleDto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _analysisService.CreateBusinessRuleAsync(rule);
            
            _logger.LogInformation("Regra de negócio criada: {RuleName}", rule.Name);
            
            return CreatedAtAction(nameof(GetBusinessRules), new { id = rule.Id }, rule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar regra de negócio");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}



