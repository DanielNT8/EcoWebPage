using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO.Community.Response
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public string ThumbnailUrl { get; set; }
        public List<string> MediaUrls { get; set; }
        public List<string> Hashtags { get; set; } // Trả về list tên hashtag

        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatar { get; set; }

        public bool IsLikedByCurrentUser { get; set; }
    }
}

