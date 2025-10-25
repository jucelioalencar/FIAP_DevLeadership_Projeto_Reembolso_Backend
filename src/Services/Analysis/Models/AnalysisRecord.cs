using System.ComponentModel.DataAnnotations;

namespace CorisSeguros.Analysis.Models;

public class AnalysisRecord
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid DocumentId { get; set; }
    
    public bool IsEligible { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Recommendation { get; set; } = string.Empty;
    
    public double Confidence { get; set; }
    
    public string? Reasoning { get; set; }
    
    public DateTime AnalysisTimestamp { get; set; }
}



