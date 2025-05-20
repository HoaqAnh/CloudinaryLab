using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cloudinary.Service;

namespace Cloudinary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CloudinaryApiController : ControllerBase
    {
        private readonly IImageService _imageService;

        public CloudinaryApiController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var result = await _imageService.UploadImageAsync(file);
            if (result == null)
                return BadRequest("Upload failed");

            return Ok(new
            {
                result.PublicId,
                result.SecureUrl,
                result.Format
            });
        }


        [HttpGet("get/{publicId}")]
        public async Task<IActionResult> Get(string publicId)
        {
            var result = await _imageService.GetImageAsync(publicId);
            if (result == null)
                return NotFound("Image not found");
            return Ok(new
            {
                result.PublicId,
                result.SecureUrl,
                result.Format
            });
        }
        [HttpPost("test")]
        public IActionResult Test()
        {
            return Ok("API is working");
        }

        [HttpPut("update/{publicId}")]
        public async Task<IActionResult> Update(string publicId, [FromForm] IFormFile file)
        {
            var result = await _imageService.UpdateImageAsync(publicId, file);
            if (result == null)
                return BadRequest("Update failed");
            return Ok(new
            {
                result.PublicId,
                result.SecureUrl,
                result.Format
            });
        }

        [HttpDelete("delete/{publicId}")]
        public async Task<IActionResult> Delete(string publicId)
        {
            var result = await _imageService.DeleteImageAsync(publicId);
            if (result.Result != "ok")
                return BadRequest("Delete failed");
            return Ok("Image deleted successfully");
        }
        [HttpGet("folder/cloudInfo")]
        public async Task<IActionResult> GetImagesFromCloudInfoFolder()
        {
            var folderName = "cloudInfo";
            var result = await _imageService.GetImagesFromFolderAsync(folderName);

            if (result == null || result.Error != null)
            {
                return StatusCode(500, $"Failed to retrieve images from folder '{folderName}'. Error: {result?.Error?.Message}");
            }

            if (result.Resources == null || !result.Resources.Any())
            {
                return NotFound($"No images found in folder '{folderName}'.");
            }

            // Trả về danh sách các thông tin ảnh cần thiết
            var images = result.Resources.Select(r => new
            {
                r.PublicId,
                SecureUrl = r.SecureUrl?.ToString(),
                r.Format,
                r.ResourceType,
                r.Type,
                r.CreatedAt
            }).ToList();

            return Ok(images);
        }
        [HttpPost("upload_to_folder")]
        public async Task<IActionResult> UploadToFolder([FromForm] IFormFile file, [FromForm] string folder)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            //if (string.IsNullOrWhiteSpace(folder))
            //{
            //    
            //}

            var result = await _imageService.UploadImageToFolderAsync(file, folder);

            if (result == null)
            {
                return BadRequest($"Upload to folder '{folder}' failed.");
            }

            return Ok(new
            {
                result.PublicId,
                SecureUrl = result.SecureUrl?.ToString(),
                result.Format,
                Folder = folder 
            });
        }

    }
}