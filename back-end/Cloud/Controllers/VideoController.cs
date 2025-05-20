using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cloudinary.Service;
using CloudinaryDotNet.Actions;

namespace Cloudinary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }
        // POST: api/video/upload
        [HttpPost("upload")]
        [ProducesResponseType(typeof(VideoUploadResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Vui lòng cung cấp một file video." });
            }

            var uploadResult = await _videoService.UploadVideoAsync(file);

            if (uploadResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi không xác định khi tải video lên." });
            }

            if (uploadResult.Error != null || uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return StatusCode((int)uploadResult.StatusCode, uploadResult); // Trả về lỗi từ Cloudinary
            }

            return Ok(uploadResult);
        }

        // POST: api/video/upload/{folderName}
        [HttpPost("upload/{folderName}")]
        [ProducesResponseType(typeof(VideoUploadResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadVideoToFolder(IFormFile file, [FromRoute] string folderName)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Vui lòng cung cấp một file video." });
            }
            if (string.IsNullOrWhiteSpace(folderName))
            {
                return BadRequest(new { message = "Tên thư mục không được để trống." });
            }

            var uploadResult = await _videoService.UploadVideoToFolderAsync(file, folderName);

            if (uploadResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi không xác định khi tải video vào thư mục." });
            }

            if (uploadResult.Error != null || uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return StatusCode((int)uploadResult.StatusCode, uploadResult);
            }

            return Ok(uploadResult);
        }

        // DELETE: api/video/{publicId}
        [HttpDelete("{publicId}")]
        [ProducesResponseType(typeof(DeletionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteVideo(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
            {
                return BadRequest(new { message = "PublicId không được để trống." });
            }

            var deletionResult = await _videoService.DeleteVideoAsync(publicId);

            if (deletionResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi không xác định khi xóa video." });
            }

            if (deletionResult.Result?.ToLower() == "not found")
            {
                return NotFound(deletionResult);
            }

            if (deletionResult.Error != null || deletionResult.Result?.ToLower() != "ok")
            {
                return BadRequest(deletionResult);
            }

            return Ok(deletionResult);
        }

        // PUT: api/video/{publicId}
        [HttpPut("{publicId}")]
        [ProducesResponseType(typeof(VideoUploadResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateVideo(string publicId, IFormFile newFile)
        {
            if (string.IsNullOrWhiteSpace(publicId))
            {
                return BadRequest(new { message = "PublicId không được để trống." });
            }
            if (newFile == null || newFile.Length == 0)
            {
                return BadRequest(new { message = "Vui lòng cung cấp file video mới." });
            }

            var updateResult = await _videoService.UpdateVideoAsync(publicId, newFile);

            if (updateResult == null)
            {
                return BadRequest(new { message = "Không thể xử lý yêu cầu cập nhật." });
            }

            if (updateResult.Error != null || updateResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return StatusCode((int)updateResult.StatusCode, updateResult);
            }

            return Ok(updateResult);
        }

        // GET: api/video/{publicId}
        [HttpGet("{publicId}")]
        [ProducesResponseType(typeof(GetResourceResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetVideo(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
            {
                return BadRequest(new { message = "PublicId không được để trống." });
            }

            var getResult = await _videoService.GetVideoAsync(publicId);

            if (getResult == null)
            {
                return NotFound(new { message = $"Video với PublicId '{publicId}' không được tìm thấy." });
            }

            if (getResult.Error != null || getResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (getResult.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound(getResult);
                }
                return StatusCode((int)getResult.StatusCode, getResult);
            }

            return Ok(getResult);
        }

        // GET: api/video/folder/{folderName}
        [HttpGet("folder/{folderName}")]
        [ProducesResponseType(typeof(ListResourcesResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)] // Thường Cloudinary trả về OK với mảng rỗng
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetVideosFromFolder(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                return BadRequest(new { message = "Tên thư mục không được để trống." });
            }

            var listResult = await _videoService.GetVideoFromFolderAsync(folderName);

            if (listResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi không xác định khi lấy danh sách video từ thư mục." });
            }

            if (listResult.Error != null || listResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return StatusCode((int)listResult.StatusCode, listResult);
            }

            // Ngay cả khi thư mục không tồn tại hoặc trống, Cloudinary thường trả về 200 OK với mảng Resources rỗng.
            // Bạn có thể quyết định trả về NotFound nếu mảng Resources rỗng, nếu muốn:
            // if (listResult.Resources == null || !listResult.Resources.Any())
            // {
            //     return NotFound(new { message = $"Không tìm thấy video nào trong thư mục '{folderName}' hoặc thư mục không tồn tại." });
            // }

            return Ok(listResult);
        }
    }
}
