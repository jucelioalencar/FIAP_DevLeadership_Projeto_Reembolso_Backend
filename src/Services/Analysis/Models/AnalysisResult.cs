namespace CorisSeguros.Analysis.Models;

public class AnalysisResult
{
    public Guid DocumentId { get; set; }
    public bool IsEligible { get; set; }
    public string Recommendation { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Reasoning { get; set; } = string.Empty;
    public List<BusinessRuleResult> BusinessRulesApplied { get; set; } = new();
    public DateTime AnalysisTimestamp { get; set; }
}

public class BusinessRuleResult
{
    public Guid RuleId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public bool Passed { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
}

public class DelayEligibilityResult
{
    public bool IsEligible { get; set; }
    public string Reason { get; set; } = string.Empty;
}



