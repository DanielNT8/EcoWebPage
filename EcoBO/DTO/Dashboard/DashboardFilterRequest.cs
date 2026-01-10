using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO.Dashboard
{
    public class DashboardFilterRequest
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
