using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CorisSeguros.Ingestion.Models;
using CorisSeguros.Ingestion.DTOs;
using CorisSeguros.Ingestion.Infrastructure;

namespace CorisSeguros.Ingestion.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReimbursementController : ControllerBase
{
    private readonly IngestionDbContext _context;
    private readonly ILogger<ReimbursementController> _logger;

    public ReimbursementController(
        IngestionDbContext context,
        ILogger<ReimbursementController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetReimbursements(
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = _context.Documents.AsQueryable();

            // Filtro de busca
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => 
                    d.FileName.Contains(search) ||
                    d.PassengerName!.Contains(search) ||
                    d.FlightNumber!.Contains(search));
            }

            // Filtro de status
            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                if (Enum.TryParse<DocumentStatus>(status, out var statusEnum))
                {
                    query = query.Where(d => d.Status == statusEnum);
                }
            }

            var totalCount = await query.CountAsync();
            
            var reimbursements = await query
                .OrderByDescending(d => d.UploadedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new ReimbursementDto
                {
                    Id = d.Id.ToString(),
                    PassengerName = d.PassengerName ?? "N/A",
                    FlightNumber = d.FlightNumber ?? "N/A",
                    FlightDate = d.UploadedAt.ToString("dd/MM/yyyy"),
                    Delay = CalculateDelay(d), // Mock calculation
                    Value = CalculateValue(d), // Mock calculation
                    OcrConfidence = CalculateOcrConfidence(d), // Mock calculation
                    Status = d.Status.ToString(),
                    UploadedAt = d.UploadedAt
                })
                .ToListAsync();

            return Ok(new PagedResult<ReimbursementDto>
            {
                Items = reimbursements,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter lista de reembolsos");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReimbursementDetails(Guid id)
    {
        try
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
            {
                return NotFound("Reembolso não encontrado");
            }

            var details = new ReimbursementDetailsDto
            {
                Id = document.Id.ToString(),
                PassengerName = document.PassengerName ?? "N/A",
                FlightNumber = document.FlightNumber ?? "N/A",
                FlightDate = document.UploadedAt.ToString("dd/MM/yyyy"),
                Delay = CalculateDelay(document),
                Value = CalculateValue(document),
                OcrConfidence = CalculateOcrConfidence(document),
                Status = document.Status.ToString(),
                UploadedAt = document.UploadedAt,
                ProcessedAt = document.ProcessedAt,
                ExtractedData = ParseExtractedData(document.ExtractedData),
                ValidationResult = ParseValidationResult(document.ValidationResult),
                AnalysisResult = ParseAnalysisResult(document.AnalysisResult)
            };

            return Ok(details);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter detalhes do reembolso {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveReimbursement(Guid id, [FromBody] ApprovalRequestDto request)
    {
        try
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
            {
                return NotFound("Reembolso não encontrado");
            }

            // Atualizar status para aprovado
            document.Status = DocumentStatus.Approved;
            document.ProcessedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("Reembolso {Id} aprovado", id);

            return Ok(new { message = "Reembolso aprovado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao aprovar reembolso {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectReimbursement(Guid id, [FromBody] RejectionRequestDto request)
    {
        try
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
            {
                return NotFound("Reembolso não encontrado");
            }

            // Atualizar status para rejeitado
            document.Status = DocumentStatus.Rejected;
            document.ProcessedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("Reembolso {Id} rejeitado: {Reason}", id, request.Reason);

            return Ok(new { message = "Reembolso rejeitado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao rejeitar reembolso {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    private static string CalculateDelay(Document document)
    {
        // Mock calculation - em produção seria baseado nos dados extraídos
        var random = new Random(document.Id.GetHashCode());
        var delay = random.Next(1, 8);
        return $"{delay}.{random.Next(0, 9)}h";
    }

    private static decimal CalculateValue(Document document)
    {
        // Mock calculation - em produção seria baseado nos dados extraídos
        var random = new Random(document.Id.GetHashCode());
        return random.Next(500, 2500);
    }

    private static int CalculateOcrConfidence(Document document)
    {
        // Mock calculation - em produção seria baseado na confiança real do OCR
        var random = new Random(document.Id.GetHashCode());
        return random.Next(85, 99);
    }

    private static object? ParseExtractedData(string? extractedData)
    {
        if (string.IsNullOrEmpty(extractedData))
            return null;

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<object>(extractedData);
        }
        catch
        {
            return null;
        }
    }

    private static object? ParseValidationResult(string? validationResult)
    {
        if (string.IsNullOrEmpty(validationResult))
            return null;

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<object>(validationResult);
        }
        catch
        {
            return null;
        }
    }

    private static object? ParseAnalysisResult(string? analysisResult)
    {
        if (string.IsNullOrEmpty(analysisResult))
            return null;

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<object>(analysisResult);
        }
        catch
        {
            return null;
        }
    }
}



