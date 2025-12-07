using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using EcoBO.Settings;
using EcoService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoService.Services
{
    public class MediaService : IMediaService
    {
        private readonly Cloudinary _cloudinary;

        public MediaService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();

                // Tạo tên file ngẫu nhiên để tránh trùng lặp: guid_tenfile
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    // Quy định tên file trên Cloudinary (Public ID)
                    PublicId = uniqueFileName,
                    Folder = "Eco_Community_Images", // Tự động tạo folder này

                    // Tự động xoay ảnh (nếu chụp bằng điện thoại) và tối ưu hóa
                    Transformation = new Transformation().Quality("auto").FetchFormat("auto")
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public async Task<VideoUploadResult> UploadVideoAsync(IFormFile file)
        {
            var uploadResult = new VideoUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();

                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = uniqueFileName,
                    Folder = "Eco_Community_Videos",
                    EagerTransforms = new List<Transformation>()
                    {
                        new Transformation().Width(300).Crop("pad")
                    }
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeleteMediaAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}
