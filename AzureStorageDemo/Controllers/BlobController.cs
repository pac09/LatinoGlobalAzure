using AzureStorageDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureStorageDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlobController : ControllerBase
{
    private readonly BlobStorageService _blobService;

    public BlobController(BlobStorageService blobService)
    {
        _blobService = blobService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        await _blobService.UploadAsync(file);
        return Ok("Uploaded successfully.");
    }

    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> Download(string fileName)
    {
        var stream = await _blobService.DownloadAsync(fileName);
        if (stream == null)
            return NotFound();

        return File(stream, "application/octet-stream", fileName);
    }

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var files = await _blobService.ListAsync();
        return Ok(files);
    }

    [HttpDelete("delete/{fileName}")]
    public async Task<IActionResult> Delete(string fileName)
    {
        var success = await _blobService.DeleteAsync(fileName);
        return success ? Ok("Deleted") : NotFound();
    }
}