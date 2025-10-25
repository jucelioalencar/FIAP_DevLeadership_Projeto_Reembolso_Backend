using CorisSeguros.Analysis.Models;
using CorisSeguros.Analysis.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CorisSeguros.Analysis.Services;

public class ReimbursementAnalysisService : IAnalysisService
{
    private readonly IBusinessRulesService _businessRulesService;
    private readonly AnalysisDbContext _context;
    private readonly ILogger<ReimbursementAnalysisService> _logger;

    public ReimbursementAnalysisService(
        IBusinessRulesService businessRulesService,
        AnalysisDbContext context,
        ILogger<ReimbursementAnalysisService> logger)
    {
        _businessRulesService = businessRulesService;
        _context = context;
        _logger = logger;
    }

    public async Task<AnalysisResult> AnalyzeReimbursementAsync(
        Guid documentId, 
        FlightData? flightData, 
        ValidationResult? validationResult)
    {
        try
        {
            _logger.LogInformation("Iniciando análise de reembolso para documento {DocumentId}", documentId);

            var analysisResult = new AnalysisResult
            {
                DocumentId = documentId,
                AnalysisTimestamp = DateTime.UtcNow
            };

            // Obter regras de negócio ativas
            var businessRules = await _businessRulesService.GetActiveRulesAsync();

            // Aplicar regras de negócio
            var ruleResults = new List<BusinessRuleResult>();
            var isEligible = true;
            var confidence = 1.0;
            var reasoning = new List<string>();

            foreach (var rule in businessRules.OrderBy(r => r.Priority))
            {
                var ruleResult = await _businessRulesService.EvaluateRuleAsync(rule, flightData, validationResult);
                ruleResults.Add(ruleResult);

                if (!ruleResult.Passed)
                {
                    isEligible = false;
                    confidence *= 0.8; // Reduzir confiança quando regras falham
                    reasoning.Add($"Regra '{rule.Name}' não passou: {ruleResult.Reason}");
                }
                else
                {
                    reasoning.Add($"Regra '{rule.Name}' passou: {ruleResult.Reason}");
                }
            }

            // Aplicar regra principal: atraso > 4 horas
            var delayEligibility = await EvaluateDelayEligibility(flightData, validationResult);
            if (!delayEligibility.IsEligible)
            {
                isEligible = false;
                confidence *= 0.5;
                reasoning.Add($"Elegibilidade por atraso: {delayEligibility.Reason}");
            }
            else
            {
                reasoning.Add($"Elegibilidade por atraso: {delayEligibility.Reason}");
            }

            // Determinar recomendação
            var recommendation = DetermineRecommendation(isEligible, confidence, ruleResults);

            analysisResult.IsEligible = isEligible;
            analysisResult.Confidence = confidence;
            analysisResult.Recommendation = recommendation;
            analysisResult.Reasoning = string.Join("; ", reasoning);
            analysisResult.BusinessRulesApplied = ruleResults;

            // Salvar resultado da análise
            await SaveAnalysisResultAsync(analysisResult);

            _logger.LogInformation("Análise concluída para documento {DocumentId}. Elegível: {IsEligible}, Confiança: {Confidence}%", 
                documentId, isEligible, confidence * 100);

            return analysisResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante análise de reembolso para documento {DocumentId}", documentId);
            throw;
        }
    }

    public async Task<List<BusinessRule>> GetBusinessRulesAsync()
    {
        return await _context.BusinessRules
            .Where(r => r.IsActive)
            .OrderBy(r => r.Priority)
            .ToListAsync();
    }

    public async Task CreateBusinessRuleAsync(BusinessRule rule)
    {
        _context.BusinessRules.Add(rule);
        await _context.SaveChangesAsync();
    }

    private async Task<DelayEligibilityResult> EvaluateDelayEligibility(
        FlightData? flightData, 
        ValidationResult? validationResult)
    {
        // Regra principal: atraso deve ser maior que 4 horas
        const int minimumDelayHours = 4;

        if (flightData == null)
        {
            return new DelayEligibilityResult
            {
                IsEligible = false,
                Reason = "Dados do voo não disponíveis"
            };
        }

        if (validationResult?.FlightAwareValidation?.DelayMinutes == null)
        {
            return new DelayEligibilityResult
            {
                IsEligible = false,
                Reason = "Informações de atraso não disponíveis"
            };
        }

        var delayHours = validationResult.FlightAwareValidation.DelayMinutes.Value / 60.0;
        
        if (delayHours >= minimumDelayHours)
        {
            return new DelayEligibilityResult
            {
                IsEligible = true,
                Reason = $"Atraso de {delayHours:F1} horas atende ao critério mínimo de {minimumDelayHours} horas"
            };
        }
        else
        {
            return new DelayEligibilityResult
            {
                IsEligible = false,
                Reason = $"Atraso de {delayHours:F1} horas não atende ao critério mínimo de {minimumDelayHours} horas"
            };
        }
    }

    private static string DetermineRecommendation(
        bool isEligible, 
        double confidence, 
        List<BusinessRuleResult> ruleResults)
    {
        if (isEligible && confidence >= 0.8)
        {
            return "APROVAR - Reembolso elegível com alta confiança";
        }
        else if (isEligible && confidence >= 0.6)
        {
            return "APROVAR COM REVISÃO - Reembolso elegível, mas requer revisão manual";
        }
        else if (!isEligible && confidence >= 0.8)
        {
            return "REJEITAR - Reembolso não elegível";
        }
        else
        {
            return "REVISÃO MANUAL - Análise manual necessária devido à baixa confiança";
        }
    }

    private async Task SaveAnalysisResultAsync(AnalysisResult result)
    {
        var analysisRecord = new AnalysisRecord
        {
            Id = Guid.NewGuid(),
            DocumentId = result.DocumentId,
            IsEligible = result.IsEligible,
            Recommendation = result.Recommendation,
            Confidence = result.Confidence,
            Reasoning = result.Reasoning,
            AnalysisTimestamp = result.AnalysisTimestamp
        };

        _context.AnalysisRecords.Add(analysisRecord);
        await _context.SaveChangesAsync();
    }
}



