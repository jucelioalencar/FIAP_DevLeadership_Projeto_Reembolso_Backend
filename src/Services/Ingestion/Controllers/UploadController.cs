using Microsoft.AspNetCore.Mvc;
using CorisSeguros.Ingestion.Services;
using CorisSeguros.Ingestion.Models;
using CorisSeguros.Ingestion.DTOs;
using CorisSeguros.Ingestion.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CorisSeguros.Ingestion.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IServiceBusService _serviceBusService;
    private readonly IngestionDbContext _context;
    private readonly ILogger<UploadController> _logger;

    public UploadController(
        IBlobStorageService blobStorageService,
        IServiceBusService serviceBusService,
        IngestionDbContext context,
        ILogger<UploadController> logger)
    {
        _blobStorageService = blobStorageService;
        _serviceBusService = serviceBusService;
        _context = context;
        _logger = logger;
    }

    [HttpPost("reimbursement")]
    public async Task<IActionResult> UploadReimbursement([FromForm] ReimbursementUploadDto uploadDto)
    {
        try
        {
            if (uploadDto.File == null || uploadDto.File.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }

            // Validar tipo de arquivo
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(uploadDto.File.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Tipo de arquivo não suportado. Apenas PDF, JPG, JPEG e PNG são permitidos.");
            }

            // Gerar ID único para o documento
            var documentId = Guid.NewGuid();
            var fileName = $"{documentId}{fileExtension}";

            // Upload para o Blob Storage
            var blobUrl = await _blobStorageService.UploadFileAsync(
                uploadDto.File.OpenReadStream(), 
                fileName, 
                uploadDto.File.ContentType);

            // Criar registro no banco de dados
            var document = new Document
            {
                Id = documentId,
                FileName = uploadDto.File.FileName,
                ContentType = uploadDto.File.ContentType,
                Size = uploadDto.File.Length,
                BlobUrl = blobUrl,
                Status = DocumentStatus.Uploaded,
                UploadedAt = DateTime.UtcNow,
                AnalystId = "analyst-001", // Mock analyst ID
                PassengerName = uploadDto.PassengerName,
                FlightNumber = uploadDto.FlightNumber
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            // Enviar mensagem para a fila de processamento
            var processMessage = new DocumentProcessMessage
            {
                DocumentId = documentId,
                BlobUrl = blobUrl,
                ContentType = uploadDto.File.ContentType,
                PassengerName = uploadDto.PassengerName,
                FlightNumber = uploadDto.FlightNumber
            };

            await _serviceBusService.SendMessageAsync("document-processing", processMessage);

            _logger.LogInformation("Reembolso {DocumentId} enviado para processamento", documentId);

            return Ok(new ReimbursementUploadResponseDto
            {
                Id = documentId.ToString(),
                Status = "Uploaded",
                Message = "Reembolso enviado para processamento com sucesso."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload do reembolso");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetUploadStatus(Guid id)
    {
        try
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
            {
                return NotFound("Reembolso não encontrado");
            }

            return Ok(new UploadStatusDto
            {
                Id = document.Id.ToString(),
                Status = document.Status.ToString(),
                LastUpdated = document.ProcessedAt ?? document.UploadedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar status do reembolso {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}



