using System.ComponentModel.DataAnnotations;

namespace CorisSeguros.Validation.Models;

public class ValidationRecord
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string FlightNumber { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string? PassengerName { get; set; }
    
    public string Status { get; set; } = string.Empty;
    
    public DateTime ValidationTimestamp { get; set; }
    
    public string? ValidationResult { get; set; } // JSON com resultado da validação
}



