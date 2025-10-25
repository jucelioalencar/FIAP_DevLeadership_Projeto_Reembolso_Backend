using CorisSeguros.Analysis.Models;

namespace CorisSeguros.Analysis.Services;

public interface IAnalysisService
{
    Task<AnalysisResult> AnalyzeReimbursementAsync(Guid documentId, FlightData? flightData, ValidationResult? validationResult);
    Task<List<BusinessRule>> GetBusinessRulesAsync();
    Task CreateBusinessRuleAsync(BusinessRule rule);
}



