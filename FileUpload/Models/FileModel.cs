using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace FileManagement.Models
{
    public class FileUpload_TTModel
    {
        [Key]
        public int FileID { get; set; }
        public string? CountryCode { get; set; }
        public string? ClientCode { get; set; }
        public int Year { get; set; }
        public string? Category { get; set; }
        public string? Location { get; set; }
        public string? FileName { get; set; }
        public string?  UploadedBy { get; set; }
        public DateTime UploadDate { get; set; }

    }

    public class Response
    {
        public bool isSuccess { get; set; }
        public string? ErrorDesc { get; set; }
    }
}
