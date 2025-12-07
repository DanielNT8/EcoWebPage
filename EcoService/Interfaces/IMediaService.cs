using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoService.Interfaces
{
    public interface IMediaService
    {
        // Upload ảnh (cho bài viết, comment, avatar)
        Task<ImageUploadResult> UploadImageAsync(IFormFile file);

        // Upload video (cho bài viết)
        Task<VideoUploadResult> UploadVideoAsync(IFormFile file);

        // Xóa file (khi user xóa bài hoặc xóa ảnh)
        Task<DeletionResult> DeleteMediaAsync(string publicId);
    }
}
