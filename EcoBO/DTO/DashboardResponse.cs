using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO
{
    public class DashboardResponse
    {

        public DashboardSummaryDto Summary { get; set; }

        public List<MonthlyRevenueDto> RevenueChart { get; set; }
    }

    public class DashboardSummaryDto
    {
        public double TotalRevenue { get; set; }        
        public int TotalUsers { get; set; }           
        public int TotalProcessedContacts { get; set; } 
    }

    public class MonthlyRevenueDto
    {
        public string Month { get; set; } 
        public double Amount { get; set; } 
    }
}
