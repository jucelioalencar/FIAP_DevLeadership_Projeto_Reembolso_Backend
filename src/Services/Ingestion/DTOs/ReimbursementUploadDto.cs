namespace CorisSeguros.Ingestion.DTOs;

public class ReimbursementUploadDto
{
    public IFormFile File { get; set; } = null!;
    public string PassengerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? FlightNumber { get; set; }
}

public class ReimbursementUploadResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class UploadStatusDto
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}



