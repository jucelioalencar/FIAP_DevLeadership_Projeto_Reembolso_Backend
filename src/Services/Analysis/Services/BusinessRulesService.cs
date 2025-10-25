using CorisSeguros.Analysis.Models;
using CorisSeguros.Analysis.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CorisSeguros.Analysis.Services;

public class BusinessRulesService : IBusinessRulesService
{
    private readonly AnalysisDbContext _context;
    private readonly ILogger<BusinessRulesService> _logger;

    public BusinessRulesService(
        AnalysisDbContext context,
        ILogger<BusinessRulesService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<BusinessRule>> GetActiveRulesAsync()
    {
        return await _context.BusinessRules
            .Where(r => r.IsActive)
            .OrderBy(r => r.Priority)
            .ToListAsync();
    }

    public async Task<BusinessRuleResult> EvaluateRuleAsync(
        BusinessRule rule, 
        FlightData? flightData, 
        ValidationResult? validationResult)
    {
        try
        {
            _logger.LogDebug("Avaliando regra: {RuleName}", rule.Name);

            var result = new BusinessRuleResult
            {
                RuleId = rule.Id,
                RuleName = rule.Name,
                EvaluatedAt = DateTime.UtcNow
            };

            // Implementar lógica de avaliação das regras
            // Por enquanto, implementação simplificada baseada no tipo de regra
            
            switch (rule.Name.ToLower())
            {
                case "delay_threshold":
                    result = await EvaluateDelayThresholdRule(rule, flightData, validationResult);
                    break;
                    
                case "passenger_validation":
                    result = await EvaluatePassengerValidationRule(rule, flightData, validationResult);
                    break;
                    
                case "flight_status":
                    result = await EvaluateFlightStatusRule(rule, flightData, validationResult);
                    break;
                    
                case "ticket_price_limit":
                    result = await EvaluateTicketPriceRule(rule, flightData, validationResult);
                    break;
                    
                default:
                    result.Passed = true;
                    result.Reason = "Regra não implementada - assumindo aprovação";
                    break;
            }

            _logger.LogDebug("Regra {RuleName} avaliada: {Passed} - {Reason}", 
                rule.Name, result.Passed, result.Reason);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao avaliar regra {RuleName}", rule.Name);
            return new BusinessRuleResult
            {
                RuleId = rule.Id,
                RuleName = rule.Name,
                Passed = false,
                Reason = $"Erro na avaliação: {ex.Message}",
                EvaluatedAt = DateTime.UtcNow
            };
        }
    }

    private async Task<BusinessRuleResult> EvaluateDelayThresholdRule(
        BusinessRule rule, 
        FlightData? flightData, 
        ValidationResult? validationResult)
    {
        var result = new BusinessRuleResult
        {
            RuleId = rule.Id,
            RuleName = rule.Name,
            EvaluatedAt = DateTime.UtcNow
        };

        if (validationResult?.FlightAwareValidation?.DelayMinutes == null)
        {
            result.Passed = false;
            result.Reason = "Informações de atraso não disponíveis";
            return result;
        }

        var delayHours = validationResult.FlightAwareValidation.DelayMinutes.Value / 60.0;
        const double minimumDelayHours = 4.0;

        if (delayHours >= minimumDelayHours)
        {
            result.Passed = true;
            result.Reason = $"Atraso de {delayHours:F1} horas atende ao critério mínimo";
        }
        else
        {
            result.Passed = false;
            result.Reason = $"Atraso de {delayHours:F1} horas não atende ao critério mínimo de {minimumDelayHours} horas";
        }

        return result;
    }

    private async Task<BusinessRuleResult> EvaluatePassengerValidationRule(
        BusinessRule rule, 
        FlightData? flightData, 
        ValidationResult? validationResult)
    {
        var result = new BusinessRuleResult
        {
            RuleId = rule.Id,
            RuleName = rule.Name,
            EvaluatedAt = DateTime.UtcNow
        };

        if (string.IsNullOrEmpty(flightData?.PassengerName))
        {
            result.Passed = false;
            result.Reason = "Nome do passageiro não disponível";
            return result;
        }

        if (validationResult?.InternalSystemValidation?.IsValid == true)
        {
            result.Passed = true;
            result.Reason = "Passageiro validado nos sistemas internos";
        }
        else
        {
            result.Passed = false;
            result.Reason = "Passageiro não encontrado nos sistemas internos";
        }

        return result;
    }

    private async Task<BusinessRuleResult> EvaluateFlightStatusRule(
        BusinessRule rule, 
        FlightData? flightData, 
        ValidationResult? validationResult)
    {
        var result = new BusinessRuleResult
        {
            RuleId = rule.Id,
            RuleName = rule.Name,
            EvaluatedAt = DateTime.UtcNow
        };

        if (validationResult?.FlightAwareValidation?.IsValid == true)
        {
            result.Passed = true;
            result.Reason = "Voo validado no FlightAware";
        }
        else
        {
            result.Passed = false;
            result.Reason = "Voo não encontrado ou inválido no FlightAware";
        }

        return result;
    }

    private async Task<BusinessRuleResult> EvaluateTicketPriceRule(
        BusinessRule rule, 
        FlightData? flightData, 
        ValidationResult? validationResult)
    {
        var result = new BusinessRuleResult
        {
            RuleId = rule.Id,
            RuleName = rule.Name,
            EvaluatedAt = DateTime.UtcNow
        };

        if (flightData?.TicketPrice == null)
        {
            result.Passed = true; // Se não há informação de preço, não rejeitar
            result.Reason = "Informação de preço não disponível - não aplicável";
            return result;
        }

        const decimal maxTicketPrice = 10000; // R$ 10.000,00

        if (flightData.TicketPrice <= maxTicketPrice)
        {
            result.Passed = true;
            result.Reason = $"Preço do bilhete (R$ {flightData.TicketPrice:F2}) dentro do limite";
        }
        else
        {
            result.Passed = false;
            result.Reason = $"Preço do bilhete (R$ {flightData.TicketPrice:F2}) excede o limite de R$ {maxTicketPrice:F2}";
        }

        return result;
    }
}



