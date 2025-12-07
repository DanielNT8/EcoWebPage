using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO.Feedback
{
    public class FeedbackFilterParam
    {
        // Search theo Message hoặc ContactInfo
        public string? Search { get; set; }

        // Filter: Status
        public string? Status { get; set; }

        // Sort field: "createdAt", "message", "contactInfo"
        public string? SortBy { get; set; } = "createdAt";

        // true = ASC, false = DESC
        public bool SortAscending { get; set; } = false;

        // Phân trang
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
