using FileManagement.DBContexts;
using FileManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Data.SqlClient;

namespace FileManagement.Repositories.Impl
{
   public class FileManagementRepository : IFileManagementRepository
    {
        private readonly FileDbContext context;

        public FileManagementRepository(FileDbContext context)
        {
            this.context = context;
        }

        public async Task<Response> InsertFileUpload_TT(FileUpload_TTModel inModel)
        {
            Response resp = new Response();
            try
            {
                var _resContext = context.FileUpload_TT.AddAsync(inModel);

                var _save = context.SaveChanges();

                resp.isSuccess = true;

            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
            {
                resp.isSuccess = false;
                resp.ErrorDesc = "Duplicate index error: The fileName already exists.";
                return resp;
            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.ErrorDesc = ex.Message;
                return await Task.FromResult(resp);
            }

            return await Task.FromResult(resp);
        }

        public async Task<List<FileUpload_TTModel>> DeleteFile(string? countryCode, string? clientCode, int? year, string? category, string? fileName)
        {
            Response resp = new Response();
            try
            {

                var query = context.FileUpload_TT.AsQueryable();

                if (!string.IsNullOrEmpty(countryCode))
                {
                    query = query.Where(i => i.CountryCode == countryCode);
                }

                if (!string.IsNullOrEmpty(clientCode))
                {
                    query = query.Where(i => i.ClientCode == clientCode);
                }

                if (year.HasValue)
                {
                    query = query.Where(i => i.Year == year);
                }

                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(i => i.Category == category);
                }

                if (!string.IsNullOrEmpty(fileName))
                {
                    query = query.Where(i => i.FileName == fileName);
                }


                var result = await query.ToListAsync();

                if (result.Any())
                {
                    context.FileUpload_TT.RemoveRange(result);
                    await context.SaveChangesAsync();
                }

                return result;

            }
            catch 
            {
                throw;
            }

        }

        public async Task<List<FileUpload_TTModel>> GetListFiles(string? countryCode, string? clientCode, int? year)
        {
            Response resp = new Response();
            try
            {

                var query = context.FileUpload_TT.AsQueryable();

                if (!string.IsNullOrEmpty(countryCode))
                {
                    query = query.Where(i => i.CountryCode == countryCode);
                }

                if (!string.IsNullOrEmpty(clientCode))
                {
                    query = query.Where(i => i.ClientCode == clientCode);
                }

                if (year.HasValue)
                {
                    query = query.Where(i => i.Year == year);
                }

                var result = await query.ToListAsync();

                return result;

            }
            catch 
            {
                throw;
            }

        }

        public async Task<List<FileUpload_TTModel>> GetFilesByKeyword(string? keyword)
        {
            Response resp = new Response();
            try
            {

                var query = context.FileUpload_TT.AsQueryable();

                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(i => EF.Functions.Like(i.FileName.ToLower(), $"%{keyword.ToLower()}%"));
                }

                var result = await query.ToListAsync();

                return result;

            }
            catch 
            {
                throw;
            }

        }

        public async Task<FileUpload_TTModel> GetUploadDetail(string fileName)
        {
            Response resp = new Response();
            try
            {

                var query = context.FileUpload_TT.AsQueryable();

                query = query.Where(i => i.FileName == fileName);


                var result = await query.FirstOrDefaultAsync();

                return result;

            }
            catch 
            {
                throw;
            }

        }

    }
}
