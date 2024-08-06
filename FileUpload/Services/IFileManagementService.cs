using FileManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileManagement.Service
{
    public interface IFileManagementService
    {
        Task<Response> InsertTableProcess(FileUpload_TTModel inModel);
        Task<List<string>> DeleteFile(string? countryCode, string? clientCode, int? year, string? category, string? fileName);
        Task<List<string>> GetListFiles(string? countryCode, string? clientCode, int? year);
        Task<List<string>> GetFilesByKeyword(string? keyword);
        Task<FileUpload_TTModel> GetUploadDetail(string fileName);
    }
}
