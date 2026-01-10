using EcoBO.DTO.Dashboard;
using EcoService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoAPI.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            // Nếu Frontend không gửi ngày (Load lần đầu), mặc định lấy 7 ngày gần nhất
            var request = new DashboardFilterRequest
            {
                FromDate = fromDate ?? DateTime.UtcNow.AddDays(-7),
                ToDate = toDate ?? DateTime.UtcNow
            };

            var data = await _dashboardService.GetDashboardStatsAsync(request);
            return Ok(data);
        }
    }
}
