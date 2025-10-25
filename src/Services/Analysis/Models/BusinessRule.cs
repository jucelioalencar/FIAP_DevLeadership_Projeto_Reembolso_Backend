using System.ComponentModel.DataAnnotations;

namespace CorisSeguros.Analysis.Models;

public class BusinessRule
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    public string Condition { get; set; } = string.Empty; // JSON ou expressão lógica
    
    [Required]
    public string Action { get; set; } = string.Empty; // Ação a ser executada
    
    public int Priority { get; set; } = 1; // Prioridade de execução (menor = maior prioridade)
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}



