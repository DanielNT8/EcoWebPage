using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO.Community.Request
{
    public class CreatePostRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string>? MediaUrls { get; set; } // List ảnh
        public string? ThumbnailUrl { get; set; }

        // Hashtag dạng chuỗi: ["#eco", "#green"]
        // Backend tự xử lý tìm ID hoặc tạo mới
        public List<string>? Hashtags { get; set; }
    }
}
