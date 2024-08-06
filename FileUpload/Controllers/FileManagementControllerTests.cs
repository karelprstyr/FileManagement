using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using FileManagement.Controllers;
using FileManagement.Service;
using FileManagement.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.HttpResults;

public class FileManagementControllerTests
{
    private readonly FileManagementController _controller;
    private readonly Mock<IFileManagementService> _mockService;
    private readonly Mock<IWebHostEnvironment> _mockEnvironment;
    private readonly Mock<IConfiguration> _mockConfiguration;

    public FileManagementControllerTests()
    {
        _mockService = new Mock<IFileManagementService>();
        _mockEnvironment = new Mock<IWebHostEnvironment>();
        _mockConfiguration = new Mock<IConfiguration>();

        _controller = new FileManagementController(
            _mockService.Object,
            _mockEnvironment.Object,
            _mockConfiguration.Object
        );
    }

    [Fact]
    public async Task UploadFile_WithValidFile_ReturnsOkResult()
    {
        var file = new FormFile(new MemoryStream(), 0, 0, "file", "ID_Client2_2023_CategoryA.txt");
        _mockEnvironment.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());

        var username = "testuser";

        _mockService.Setup(s => s.InsertTableProcess(It.IsAny<FileUpload_TTModel>()))
                    .ReturnsAsync(new Response { isSuccess = true });

        var result = await _controller.UploadFile(file, username);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("File uploaded successfully", ((dynamic)okResult.Value).message);
    }

    [Fact]
    public async Task UploadFile_WithValidFile_ReturnsBadRequest()
    {
        var file = new FormFile(new MemoryStream(), 0, 0, "file", "ID_Client2_2023_CategoryA.txt");
        _mockEnvironment.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());


        _mockService.Setup(s => s.InsertTableProcess(It.IsAny<FileUpload_TTModel>()))
                    .ReturnsAsync(new Response { isSuccess = true });

        var result = await _controller.UploadFile(file, null);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Username is required from header.", badRequestResult.Value);
    }

    [Fact]
    public async Task GetListFiles_WithValidParametersAndFiles_ReturnsOk()
    {
        var countryCode = "ID";
        var clientCode = "Client2";
        var year = 2023;

        var fileNames = new List<string> { "ID_Client2_2023_CategoryA.txt", "ID_Client2_2023_CategoryB.txt" };
        _mockService.Setup(s => s.GetListFiles(countryCode, clientCode, year))
                    .ReturnsAsync(fileNames);

        var result = await _controller.GetListFiles(countryCode, clientCode, year);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultValue = okResult.Value as dynamic;
        Assert.Equal(fileNames, resultValue.FileNames as List<string>);
    }

    [Fact]
    public async Task GetListFiles_WithNoParameters_ReturnsBadRequest()
    {
        string? countryCode = null;
        string? clientCode = null;
        int? year = null;

        var result = await _controller.GetListFiles(countryCode, clientCode, year);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No Parameter Detected", badRequestResult.Value);
    }

    [Fact]
    public async Task GetListFiles_WithValidParametersButNoFiles_ReturnsNotFound()
    {
        string? countryCode = null;
        string? clientCode = null;
        int? year = 99999999;

        _mockService.Setup(s => s.GetListFiles(countryCode, clientCode, year))
                    .ReturnsAsync(new List<string>());

        var result = await _controller.GetListFiles(countryCode, clientCode, year);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No data found for the specified criteria.", ((dynamic)notFoundResult.Value).Message);
    }


    [Fact]
    public async Task DeleteFile_WithValidParameters_ReturnsOkResult()
    {
        var countryCode = "ID";
        var clientCode = "Client2";
        var year = 2023;
        var category = "CategoryA";
        var fileName = "ID_Client2_2023_CategoryA.txt";

        _mockService.Setup(s => s.DeleteFile(countryCode, clientCode, year, category, fileName))
                    .ReturnsAsync(new List<string> { "deletedfile.txt" });

        var result = await _controller.DeleteFile(countryCode, clientCode, year, category, fileName);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("File deleted successfully", ((dynamic)okResult.Value).message);
    }

    [Fact]
    public async Task DeleteFile_WithNoParameters_ReturnsBadRequest()
    {
        string? countryCode = null;
        string? clientCode = null;
        int? year = null;
        string? category = null;
        string? fileName = null;

        var result = await _controller.DeleteFile(countryCode, clientCode, year, category, fileName);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No Parameter Detected", badRequestResult.Value);
    }

    [Fact]
    public async Task DeleteFile_WithInvalidParameters_ReturnsNotFound()
    {
        string? countryCode = null;
        string? clientCode = null;
        int? year = 9999999;
        string? category = null;
        string? fileName = null;

        _mockService.Setup(s => s.DeleteFile(countryCode, clientCode, year, category, fileName))
                    .ReturnsAsync(new List<string>());

        var result = await _controller.DeleteFile(countryCode, clientCode, year, category, fileName);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No data found for the specified criteria.", ((dynamic)notFoundResult.Value).Message);
    }

    [Fact]
    public async Task GetFilesByKeyword_WithValidKeyword_ReturnsOk()
    {
        var keyword = "ID";
        var fileNames = new List<string> { "ID_Client2_2023_CategoryA.txt", "ID_Client2_2023_CategoryB.txt" };
        _mockService.Setup(s => s.GetFilesByKeyword(keyword))
                    .ReturnsAsync(fileNames);

        var result = await _controller.GetFilesByKeyword(keyword);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultValue = okResult.Value as dynamic;
        Assert.Equal(fileNames, resultValue.FileNames as List<string>);
    }

    [Fact]
    public async Task GetFilesByKeyword_WithInvalidKeyword_ReturnsNotFound()
    {
        var keyword = "invalid";
        var fileNames = new List<string>();
        _mockService.Setup(s => s.GetFilesByKeyword(keyword))
                    .ReturnsAsync(fileNames);

        var result = await _controller.GetFilesByKeyword(keyword);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var resultValue = notFoundResult.Value as dynamic;
        Assert.Equal("No data found for the specified criteria.", resultValue.Message);
    }

    [Fact]
    public async Task GetUploadDetail_WithValidFileName_ReturnsOk()
    {
        var fileName = "testfile.txt";
        var fileUploadDetail = new FileUpload_TTModel
        {
            UploadedBy = "testuser",
            UploadDate = System.DateTime.UtcNow,
            Location = "path/to/file"
        };
        _mockService.Setup(s => s.GetUploadDetail(fileName))
                    .ReturnsAsync(fileUploadDetail);

        var result = await _controller.getUploadDetail(fileName);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultValue = okResult.Value as dynamic;
        Assert.Equal(fileUploadDetail.UploadedBy, resultValue.UploadedBy);
        Assert.Equal(fileUploadDetail.UploadDate, resultValue.UploadDate);
        Assert.Equal(fileUploadDetail.Location, resultValue.Location);
    }

    [Fact]
    public async Task GetUploadDetail_WithInvalidFileName_ReturnsNotFound()
    {
        var fileName = "invalidfile.txt";
        _mockService.Setup(s => s.GetUploadDetail(fileName))
                    .ReturnsAsync((FileUpload_TTModel)null);

        var result = await _controller.getUploadDetail(fileName);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var resultValue = notFoundResult.Value as dynamic;
        Assert.Equal("No data found for the specified criteria.", resultValue.Message);
    }

}
