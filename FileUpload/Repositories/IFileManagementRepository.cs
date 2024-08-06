using FileManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileManagement.Repositories
{
    public interface IFileManagementRepository
    {
        Task<Response> InsertFileUpload_TT(FileUpload_TTModel inModel);
        Task<List<FileUpload_TTModel>> DeleteFile(string? countryCode, string? clientCode, int? year, string? category, string? fileName);
        Task<List<FileUpload_TTModel>> GetListFiles(string? countryCode, string? clientCode, int? year);
        Task<List<FileUpload_TTModel>> GetFilesByKeyword(string? keyword);
        Task<FileUpload_TTModel> GetUploadDetail(string fileName);
    }
}
