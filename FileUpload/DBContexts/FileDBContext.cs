using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using FileManagement.Models;

namespace FileManagement.DBContexts
{
    public class FileDbContext : DbContext
    {
        public FileDbContext(DbContextOptions<FileDbContext> options) : base(options) { }

        public DbSet<FileUpload_TTModel> FileUpload_TT { get; set; }
    }
}
