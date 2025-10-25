namespace CorisSeguros.Ingestion.DTOs;

public class ReimbursementDto
{
    public string Id { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string FlightDate { get; set; } = string.Empty;
    public string Delay { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public int OcrConfidence { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class ReimbursementDetailsDto
{
    public string Id { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string FlightDate { get; set; } = string.Empty;
    public string Delay { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public int OcrConfidence { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public object? ExtractedData { get; set; }
    public object? ValidationResult { get; set; }
    public object? AnalysisResult { get; set; }
}

public class ApprovalRequestDto
{
    public string? Comment { get; set; }
}

public class RejectionRequestDto
{
    public string Reason { get; set; } = string.Empty;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}



