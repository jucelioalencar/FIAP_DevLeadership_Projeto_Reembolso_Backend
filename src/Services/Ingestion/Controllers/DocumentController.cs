using Microsoft.AspNetCore.Mvc;
using CorisSeguros.Ingestion.Services;
using CorisSeguros.Ingestion.Models;
using CorisSeguros.Ingestion.DTOs;

namespace CorisSeguros.Ingestion.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IServiceBusService _serviceBusService;
    private readonly ILogger<DocumentController> _logger;

    public DocumentController(
        IBlobStorageService blobStorageService,
        IServiceBusService serviceBusService,
        ILogger<DocumentController> logger)
    {
        _blobStorageService = blobStorageService;
        _serviceBusService = serviceBusService;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadDocument([FromForm] DocumentUploadDto uploadDto)
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
                AnalystId = uploadDto.AnalystId,
                PassengerName = uploadDto.PassengerName,
                FlightNumber = uploadDto.FlightNumber
            };

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

            _logger.LogInformation("Documento {DocumentId} enviado para processamento", documentId);

            return Ok(new DocumentUploadResponseDto
            {
                DocumentId = documentId,
                Status = "Uploaded",
                Message = "Documento enviado para processamento com sucesso."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload do documento");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("{documentId}/status")]
    public async Task<IActionResult> GetDocumentStatus(Guid documentId)
    {
        try
        {
            // Aqui você implementaria a consulta ao banco de dados
            // Por enquanto, retornando um status mock
            return Ok(new DocumentStatusDto
            {
                DocumentId = documentId,
                Status = "Processing",
                LastUpdated = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar status do documento {DocumentId}", documentId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}

