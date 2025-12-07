using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO.Community.Request
{
    public class CreateCommentRequest
    {
        public Guid PostId { get; set; }
        public string Content { get; set; }
        public string? MediaUrl { get; set; }
        public Guid? ParentCommentId { get; set; }
    }
}
