using EcoService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoAPI.Controllers
{
    [Route("api/media")]
    [ApiController]
    [Authorize] // Chỉ user đăng nhập mới được upload để tránh spam
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;

        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        // POST: api/media/upload-image
        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file ảnh");

            // Validate đuôi file (chỉ cho phép ảnh)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Định dạng file không hợp lệ. Chỉ chấp nhận ảnh.");

            var result = await _mediaService.UploadImageAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            return Ok(new
            {
                url = result.SecureUrl.AbsoluteUri,
                publicId = result.PublicId // Lưu cái này nếu muốn xóa ảnh sau này
            });
        }

        // POST: api/media/upload-video
        [HttpPost("video")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("Vui lòng chọn file video");

            // Validate đuôi file video
            var allowedExtensions = new[] { ".mp4", ".mov", ".avi", ".webm" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Định dạng file không hợp lệ. Chỉ chấp nhận video.");

            // Giới hạn dung lượng (Ví dụ: 50MB)
            if (file.Length > 50 * 1024 * 1024)
                return BadRequest("Video không được quá 50MB");

            var result = await _mediaService.UploadVideoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            return Ok(new
            {
                url = result.SecureUrl.AbsoluteUri,
                publicId = result.PublicId
            });
        }
    }
}
