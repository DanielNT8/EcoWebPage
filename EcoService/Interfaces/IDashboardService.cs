using EcoBO.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoService.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardResponse> GetDashboardStatsAsync();
    }
}
