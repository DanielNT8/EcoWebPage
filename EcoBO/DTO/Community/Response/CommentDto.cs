using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO.Community.Response
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string Content { get; set; }
        public string? MediaUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        // Người bình luận
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }

        // Reply (Chỉ hỗ trợ 1 cấp reply cho đơn giản, hoặc đệ quy tùy UI)
        public Guid? ParentCommentId { get; set; }

    }
}
