using CaseBattleBackend.Dtos;
using CaseBattleBackend.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;

namespace CaseBattleBackend.Services;

public class GoogleDriveService : IStorageService
{
    private readonly string _googleDriveFolderId;
    private readonly string _googleBaseFileUrl;
    private readonly DriveService _driveService;

    public GoogleDriveService(IConfiguration configuration)
    {
        _googleDriveFolderId =
            configuration["Google:FolderId"] ?? throw new InvalidOperationException();
        _googleBaseFileUrl = configuration["Google:BaseFileUrl"] ?? throw new InvalidOperationException();
        var googleDriveCredentialsPath = configuration["Google:Credentials"] ?? throw new InvalidOperationException();

        using var stream = new FileStream(googleDriveCredentialsPath, FileMode.Open, FileAccess.Read);
        var credential = GoogleCredential.FromStream(stream).CreateScoped(DriveService.ScopeConstants.DriveFile);

        _driveService = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Telegram Online Store"
        });
    }

    public async Task<FileDto> UploadFile(IFormFile file, string name)
    {
        try
        {
            var allowedExtensions = new[] { ".png", ".gif", ".webp", ".jpeg", ".jpg" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                throw new Exception("Unsupported file extension");

            var fileName = $"{name}{extension}";

            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = fileName,
                MimeType = file.ContentType,
                Parents = [_googleDriveFolderId]
            };

            FilesResource.CreateMediaUpload request;
            await using (var fileStream = file.OpenReadStream())
            {
                request = _driveService.Files.Create(fileMetadata, fileStream, file.ContentType);
                request.Fields = "id";
                request.SupportsAllDrives = true;
                var uploadResult = await request.UploadAsync();

                if (uploadResult.Status != UploadStatus.Completed || request.ResponseBody?.Id == null)
                    throw new Exception("File upload failed or fileId is missing.");
            }

            var fileId = request.ResponseBody?.Id;
            if (string.IsNullOrEmpty(fileId))
                throw new Exception("File ID is null after upload.");

            var permission = new Google.Apis.Drive.v3.Data.Permission { Role = "reader", Type = "anyone" };

            await _driveService.Permissions.Create(permission, fileId).ExecuteAsync();

            return new FileDto(name, fileId, $"{_googleBaseFileUrl}{fileId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading file: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteFileAsync(string fileId)
    {
        try
        {
            await _driveService.Files.Delete(fileId).ExecuteAsync();
        }
        catch (Google.GoogleApiException e) when (e.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine($"File ID {fileId} not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting {fileId}: {ex.Message}");
        }
    }

    public Task<Uri> GetFileUrl(string? id)
    {
        return Task.FromResult(new Uri($"{_googleBaseFileUrl}{id}"));
    }
}