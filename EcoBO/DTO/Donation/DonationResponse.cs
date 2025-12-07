using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO.Donation
{
    public class DonationResponse
    {
        public string PaymentLinkId { get; set; } = null!;
        public string CheckoutUrl { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = "Pending";
        public string OrderCode { get; set; }
    }
}