namespace CorisSeguros.Ingestion.Services;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<Stream> DownloadFileAsync(string blobUrl);
    Task DeleteFileAsync(string blobUrl);
}

