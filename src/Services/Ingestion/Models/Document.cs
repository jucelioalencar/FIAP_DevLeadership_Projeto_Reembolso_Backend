using System.ComponentModel.DataAnnotations;

namespace CorisSeguros.Ingestion.Models;

public class Document
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;
    
    public long Size { get; set; }
    
    [Required]
    public string BlobUrl { get; set; } = string.Empty;
    
    public DocumentStatus Status { get; set; }
    
    public DateTime UploadedAt { get; set; }
    
    public DateTime? ProcessedAt { get; set; }
    
    [Required]
    public string AnalystId { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string? PassengerName { get; set; }
    
    [MaxLength(50)]
    public string? FlightNumber { get; set; }
    
    public string? ExtractedData { get; set; } // JSON com dados extraídos
    public string? ValidationResult { get; set; } // JSON com resultado da validação
    public string? AnalysisResult { get; set; } // JSON com resultado da análise
}

public enum DocumentStatus
{
    Uploaded,
    Processing,
    OCRCompleted,
    ValidationCompleted,
    AnalysisCompleted,
    Approved,
    Rejected,
    Error
}

