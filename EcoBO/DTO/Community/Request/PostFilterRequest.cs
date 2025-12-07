using EcoBO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO.Community.Request
{
    public class PostFilterRequest : BaseFilterRequest
    {
        public Guid? UserId { get; set; }
        public string? Status { get; set; } = "PUBLISHED";
        public string? Tag { get; set; } // Tìm theo tên hashtag (vd: "eco")
    }
}
