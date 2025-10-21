using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO
{
    public class DashboardResponse
    {
        public double TotalDonationAmount { get; set; }
        public double MaxDonationAmount { get; set; }
        public double MinDonationAmount { get; set; }
        public double AverageDonation30Days { get; set; }
        public int DonationCount { get; set; }
        public int TotalCSRCount { get; set; }
        public double TodayDonationAmount { get; set; }
    }
}
