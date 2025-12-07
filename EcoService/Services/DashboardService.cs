using EcoBO.DTO;
using EcoBO.DTO.Contact;
using EcoRepository.Interfaces;
using EcoRepository.Repositories;
using EcoService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoService.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ITransactionHistoryRepository _transactionRepo;
        private readonly IContactRepository _contactRepo;
        private readonly IUserRepository _userRepo;

        public DashboardService(
            ITransactionHistoryRepository transactionRepo,
            IContactRepository contactRepo,
            IUserRepository userRepo)
        {
            _transactionRepo = transactionRepo;
            _contactRepo = contactRepo;
            _userRepo = userRepo;
        }

        public async Task<DashboardResponse> GetDashboardStatsAsync()
        {
            // 1. Lấy dữ liệu Transaction đã PAID
            var paidTransactions = await _transactionRepo.GetSuccessTransactionsAsync();

            // 2. Tính toán Summary
            var summary = new DashboardSummaryDto
            {
                TotalRevenue = paidTransactions.Sum(t => t.Amount ?? 0),
                TotalUsers = await _userRepo.CountUsersAsync(),
                TotalProcessedContacts = await _contactRepo.CountContactsByStatusAsync("Done")
            };

            // 3. Xử lý biểu đồ (Revenue Chart) - 6 tháng gần nhất
            var revenueChart = new List<MonthlyRevenueDto>();
            var today = DateTime.UtcNow;

            for (int i = 5; i >= 0; i--) // Loop 6 tháng (từ tháng -5 đến tháng hiện tại)
            {
                var targetMonth = today.AddMonths(-i);
                var monthLabel = targetMonth.ToString("MM/yyyy");

                // Tính tổng tiền của tháng đó
                var revenue = paidTransactions
                    .Where(t => t.DateTrade.HasValue &&
                                t.DateTrade.Value.Month == targetMonth.Month &&
                                t.DateTrade.Value.Year == targetMonth.Year)
                    .Sum(t => t.Amount ?? 0);

                revenueChart.Add(new MonthlyRevenueDto
                {
                    Month = monthLabel,
                    Amount = revenue
                });
            }

            return new DashboardResponse
            {
                Summary = summary,
                RevenueChart = revenueChart
            };
        }
    }
}
