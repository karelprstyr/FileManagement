using FileManagement.DBContexts;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using FileManagement.Models;
using FileManagement.Service;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Text;
using Microsoft.Extensions.Configuration;


namespace FileManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileManagementController : ControllerBase
    {
        private readonly IFileManagementService _service;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public FileManagementController(IFileManagementService service, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _service = service;
            _environment = environment;
            _configuration = configuration;
        }

        [HttpPost("uploadFile")]
        [Authorize]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromHeader(Name = "Username")] string username)
        {
            //if (file == null || file.Length == 0)
            //    return BadRequest("No file uploaded.");

            if (string.IsNullOrEmpty(username)) 
            {
                return BadRequest("Username is required from header.");
            }

            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var fileParts = fileName.Split('_');
            if (fileParts.Length != 4)
                return BadRequest("Invalid file name format.");

            var countryCode = fileParts[0];
            var clientCode = fileParts[1];
            if (!int.TryParse(fileParts[2], out var year))
                return BadRequest("Invalid year in file name.");
            var category = fileParts[3];

            // Create directory path
            var directoryPath = Path.Combine(_environment.ContentRootPath, "SavedFiles", countryCode, clientCode, year.ToString(), category);
            Directory.CreateDirectory(directoryPath);

            // Save metadata to the database
            var FileUploadModel = new FileUpload_TTModel
            {
                CountryCode = countryCode,
                ClientCode = clientCode,
                Year = year,
                Location = directoryPath, // Assuming category maps to destination location
                Category = category,
                FileName = file.FileName,
                UploadDate = DateTime.UtcNow,
                UploadedBy = username
            };

            Response resp = new Response();
            resp = await _service.InsertTableProcess(FileUploadModel);
            if (!resp.isSuccess)
            {
                if (resp.ErrorDesc.Contains("Duplicate index error"))
                {
                    return Conflict(new { message = resp.ErrorDesc });
                }
                else
                {
                    return StatusCode(500, new { message = resp.ErrorDesc });
                }
            }


            // Generate complete file path
            var filePath = Path.Combine(directoryPath, file.FileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { message = "File uploaded successfully", FileUploadModel });
        }

        [HttpPost("deleteFile")]
        [Authorize]
        public async Task<IActionResult> DeleteFile(string? countryCode, string? clientCode, int? year, string? category, string? fileName)
        {
            if (countryCode == null && clientCode == null && year == null && category == null && fileName == null )
            {
                return BadRequest("No Parameter Detected");
            }
            // Process the parameters
            List<string> resp = new List<string>();
            resp = await _service.DeleteFile(countryCode, clientCode, year,category,fileName);

            if (resp == null || resp.Count == 0)
            {
                return NotFound(new { Message = "No data found for the specified criteria." });
            }


            return Ok(new { message = "File deleted successfully", deletedFile = resp});
        }

        [HttpGet("getListFiles")]
        [Authorize]
        public async Task<IActionResult> GetListFiles(string? countryCode, string? clientCode, int? year)
        {

            if (countryCode == null && clientCode == null && year == null)
            {
                return BadRequest("No Parameter Detected");
            }
            // Process the parameters
            List<string> resp = new List<string>();
            resp = await _service.GetListFiles(countryCode,clientCode,year);

            if (resp == null || resp.Count == 0)
            {
                return NotFound(new { Message = "No data found for the specified criteria." });
            }

            return Ok(new { FileNames =  resp });
        }

        [HttpGet("getFilesByKeyword")]
        [Authorize]
        public async Task<IActionResult> GetFilesByKeyword(string? keyword)
        {

            // Process the parameters
            List<string> resp = new List<string>();
            resp = await _service.GetFilesByKeyword(keyword);

            if (resp == null || resp.Count == 0)
            {
                return NotFound(new { Message = "No data found for the specified criteria." });
            }

            return Ok(new { FileNames = resp });
        }

        [HttpGet("getUploadDetail")]
        [Authorize]
        public async Task<IActionResult> getUploadDetail(string fileName)
        {

            // Process the parameters
            FileUpload_TTModel resp = new FileUpload_TTModel();
            resp = await _service.GetUploadDetail(fileName);

            if (resp == null )
            {
                return NotFound(new { Message = "No data found for the specified criteria." });
            }

            return Ok(new { resp.UploadedBy, resp.UploadDate, resp.Location });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            if (login.Username == "admin" && login.Password == "admin")
            {
                var token = GenerateToken(login.Username);
                return Ok(new { token });
            }
            return Unauthorized();
        }

        private string GenerateToken(string username)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(24),
                Issuer = _configuration["Jwt:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
