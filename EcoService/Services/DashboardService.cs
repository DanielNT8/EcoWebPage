using EcoBO.DTO.Dashboard;
using EcoRepository.Interfaces;
using EcoService.Interfaces;

namespace EcoService.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ITransactionHistoryRepository _tranRepo;
        private readonly IUserRepository _userRepo;
        private readonly IContactRepository _contactRepo;

        public DashboardService(
            ITransactionHistoryRepository tranRepo,
            IUserRepository userRepo,
            IContactRepository contactRepo)
        {
            _tranRepo = tranRepo;
            _userRepo = userRepo;
            _contactRepo = contactRepo;
        }

        public async Task<DashboardResponse> GetDashboardStatsAsync(DashboardFilterRequest request)
        {
            // 1. Chuẩn hóa ngày giờ
            var currentFrom = request.FromDate.Date;
            var currentTo = request.ToDate.Date.AddDays(1).AddTicks(-1);


            // 2. Tính toán "Kỳ Trước"
            var daysDiff = (currentTo - currentFrom).TotalDays;
            var prevTo = currentFrom.AddSeconds(-1);
            var prevFrom = prevTo.AddDays(-daysDiff);

            // --- SỬA LẠI ĐOẠN NÀY: Dùng await tuần tự thay vì Task.WhenAll ---

            // Lấy Revenue
            var revCurr = await _tranRepo.GetTotalRevenueAsync(currentFrom, currentTo);
            var revPrev = await _tranRepo.GetTotalRevenueAsync(prevFrom, prevTo);

            // Lấy User
            var userCurr = await _userRepo.CountNewUsersAsync(currentFrom, currentTo);
            var userPrev = await _userRepo.CountNewUsersAsync(prevFrom, prevTo);

            // Lấy Contact
            var contactCurr = await _contactRepo.CountContactsByStatusAsync("Done", currentFrom, currentTo);
            var contactPrev = await _contactRepo.CountContactsByStatusAsync("Done", prevFrom, prevTo);

            // Lấy Chart
            var chartData = await _tranRepo.GetRevenueChartAsync(currentFrom, currentTo);

            // ------------------------------------------------------------------

            // 3. Xử lý dữ liệu biểu đồ (Fill Missing Dates)
            var finalChartData = FillMissingDates(chartData, currentFrom, currentTo);

            // 4. Trả về kết quả
            return new DashboardResponse
            {
                TotalRevenue = new MetricItem { Value = revCurr, PreviousValue = revPrev },
                NewUsers = new MetricItem { Value = userCurr, PreviousValue = userPrev },
                ProcessedContacts = new MetricItem { Value = contactCurr, PreviousValue = contactPrev },
                RevenueChart = finalChartData
            };
        }

        // Hàm helper giữ nguyên
        private List<ChartDataPoint> FillMissingDates(List<ChartDataPoint> data, DateTime from, DateTime to)
        {
            var result = new List<ChartDataPoint>();
            var isMonthly = (to - from).TotalDays > 31;

            if (isMonthly)
            {
                var current = new DateTime(from.Year, from.Month, 1);
                while (current <= to)
                {
                    var existing = data.FirstOrDefault(x => x.Date.Year == current.Year && x.Date.Month == current.Month);
                    result.Add(new ChartDataPoint
                    {
                        Date = current,
                        Value = existing?.Value ?? 0,
                        Label = current.ToString("MM/yyyy")
                    });
                    current = current.AddMonths(1);
                }
            }
            else
            {
                for (var day = from; day <= to; day = day.AddDays(1))
                {
                    var existing = data.FirstOrDefault(x => x.Date.Date == day.Date);
                    result.Add(new ChartDataPoint
                    {
                        Date = day,
                        Value = existing?.Value ?? 0,
                        Label = day.ToString("dd/MM")
                    });
                }
            }
            return result;
        }
    }
}