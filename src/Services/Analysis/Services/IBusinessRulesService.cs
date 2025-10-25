using CorisSeguros.Analysis.Models;

namespace CorisSeguros.Analysis.Services;

public interface IBusinessRulesService
{
    Task<List<BusinessRule>> GetActiveRulesAsync();
    Task<BusinessRuleResult> EvaluateRuleAsync(BusinessRule rule, FlightData? flightData, ValidationResult? validationResult);
}



