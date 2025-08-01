using CaseBattleBackend.Dtos;

namespace CaseBattleBackend.Interfaces;

public interface IStorageService
{
    Task<FileDto> UploadFile(IFormFile file, string name);
    Task DeleteFile(string fileId);
    Task<Uri> GetFileUrl(string? id);
}