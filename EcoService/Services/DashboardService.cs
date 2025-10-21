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
        private readonly TransactionHistoryRepository _transactionRepo;
        private readonly IContactRepository _contactRepo;

        public DashboardService(TransactionHistoryRepository transactionRepo, IContactRepository contactRepo)
        {
            _transactionRepo = transactionRepo;
            _contactRepo = contactRepo;
        }

        public async Task<DashboardResponse> GetDashboardDataAsync()
        {
            // ✅ Lấy toàn bộ giao dịch
            var transactions = await _transactionRepo.GetAllTransactionsAsync();
            var validTransactions = transactions
                .Where(t => t.Status == "Pending" && t.Amount.HasValue)
                .ToList();

            // ✅ 1. Tổng tiền donation
            double totalDonation = validTransactions.Sum(t => t.Amount ?? 0);

            // ✅ 2. Số giao dịch thành công
            int donationCount = validTransactions.Count;

            // ✅ 3. Giao dịch lớn nhất và nhỏ nhất
            double maxDonation = validTransactions.Any() ? validTransactions.Max(t => t.Amount ?? 0) : 0;
            double minDonation = validTransactions.Any() ? validTransactions.Min(t => t.Amount ?? 0) : 0;

            // ✅ 4. Trung bình donation trong 30 ngày
            DateTime threshold = DateTime.Today.AddDays(-30);
            var last30Days = validTransactions
                .Where(t => t.DateTrade.HasValue && t.DateTrade.Value >= threshold)
                .ToList();

            double average30Days = last30Days.Any()
                ? last30Days.Average(t => t.Amount ?? 0)
                : 0;

            // ✅ 5. Tổng tiền hôm nay
            DateTime today = DateTime.Today;
            double todayDonation = validTransactions
                .Where(t => t.DateTrade.HasValue && t.DateTrade.Value.Date == today)
                .Sum(t => t.Amount ?? 0);

            // ✅ 6. Tổng số CSR có status = Done
            var filter = new ContactFilterParam
            {
                Status = "Done"
            };

            var contactsResult = await _contactRepo.GetContactsAsync(filter);
            int totalCSR = contactsResult.Items.Count();

            // ✅ Trả về dữ liệu tổng hợp
            return new DashboardResponse
            {
                TotalDonationAmount = totalDonation,
                MaxDonationAmount = maxDonation,
                MinDonationAmount = minDonation,
                AverageDonation30Days = Math.Round(average30Days, 2),
                DonationCount = donationCount,
                TotalCSRCount = totalCSR,
                TodayDonationAmount = todayDonation
            };
        }
    }
}
