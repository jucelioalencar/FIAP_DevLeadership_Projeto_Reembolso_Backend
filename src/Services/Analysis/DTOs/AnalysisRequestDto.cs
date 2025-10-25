using CorisSeguros.Analysis.Models;

namespace CorisSeguros.Analysis.DTOs;

public class AnalysisRequestDto
{
    public Guid DocumentId { get; set; }
    public FlightData? FlightData { get; set; }
    public ValidationResult? ValidationResult { get; set; }
}

public class AnalysisResponseDto
{
    public Guid DocumentId { get; set; }
    public bool IsEligible { get; set; }
    public string Recommendation { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Reasoning { get; set; } = string.Empty;
    public List<BusinessRuleResult> BusinessRulesApplied { get; set; } = new();
    public DateTime AnalysisTimestamp { get; set; }
}

public class BusinessRuleDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public int Priority { get; set; } = 1;
    public bool IsActive { get; set; } = true;
}



