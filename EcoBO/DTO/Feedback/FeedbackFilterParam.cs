using EcoBO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO.Feedback
{
    public class FeedbackFilterParam : BaseFilterRequest
    {
        // Filter: Status
        public string? Status { get; set; }

    }
}
