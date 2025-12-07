using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO.Donation
{
    public class DonationHistoryDto
    {
        public double? Amount { get; set; }
        public string Description { get; set; } 
        public string OrderCode { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
