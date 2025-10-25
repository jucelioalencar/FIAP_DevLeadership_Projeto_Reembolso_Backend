using System.ComponentModel.DataAnnotations;

namespace CorisSeguros.Ingestion.DTOs;

public class DocumentUploadDto
{
    [Required]
    public IFormFile File { get; set; } = null!;
    
    [Required]
    public string AnalystId { get; set; } = string.Empty;
    
    public string? PassengerName { get; set; }
    
    public string? FlightNumber { get; set; }
}

public class DocumentUploadResponseDto
{
    public Guid DocumentId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class DocumentStatusDto
{
    public Guid DocumentId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}

