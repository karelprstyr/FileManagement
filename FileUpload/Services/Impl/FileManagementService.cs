using FileManagement.DBContexts;
using FileManagement.Models;
using FileManagement.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using FileManagement.Service;

namespace FileManagement.Services.Impl
{
   public class FileManagementService : IFileManagementService
    {
        private readonly IFileManagementRepository _fileRepository;
        
        public FileManagementService(IFileManagementRepository iFileManagementRepository)
        {
            this._fileRepository = iFileManagementRepository;
        }

        public async Task<Response> InsertTableProcess(FileUpload_TTModel inModel)
        {
            Response resp = new Response();
            resp = await this._fileRepository.InsertFileUpload_TT(inModel);
            return resp;
        }

        public async Task<List<string>> DeleteFile(string? countryCode, string? clientCode, int? year, string? category, string? fileName)
        {
            List<FileUpload_TTModel> result = new List<FileUpload_TTModel>();
            List<string> resp = new List<string>();
            result = await this._fileRepository.DeleteFile(countryCode, clientCode, year, category, fileName);

            foreach (FileUpload_TTModel n in result)
            {
                try { 
                    var directoryPath = n.Location ;
                    var filePath = Path.Combine(directoryPath, n.FileName);

                    System.IO.File.Delete(filePath);

                }
                catch
                {
                    throw;
                }


                resp.Add(n.FileName);
            }

            return resp;
        }

        public async Task<List<string>> GetListFiles(string? countryCode, string? clientCode, int? year)
        {
            List<FileUpload_TTModel> result = new List<FileUpload_TTModel>();
            List<string> resp = new List<string>();
            result = await this._fileRepository.GetListFiles(countryCode, clientCode, year);

            foreach (FileUpload_TTModel n in result)
            {
                resp.Add(n.FileName);
            }

            return resp;
        }

        public async Task<List<string>> GetFilesByKeyword(string? keyword)
        {
            List<FileUpload_TTModel> result = new List<FileUpload_TTModel>();
            List<string> resp = new List<string>();
            result = await this._fileRepository.GetFilesByKeyword(keyword);

            foreach (FileUpload_TTModel n in result)
            {
                resp.Add(n.FileName);
            }

            return resp;
        }

        public async Task<FileUpload_TTModel> GetUploadDetail(string fileName)
        {
            FileUpload_TTModel result = new FileUpload_TTModel();
            List<string> resp = new List<string>();
            result = await this._fileRepository.GetUploadDetail(fileName);


            return result;
        }


    }
}
