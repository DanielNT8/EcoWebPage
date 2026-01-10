using EcoBO.DTO.Dashboard;
using EcoBO.Models;
using EcoRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoRepository.Repositories
{
    public class TransactionHistoryRepository : ITransactionHistoryRepository
    {
        private readonly EcoDbContext _context;

        public TransactionHistoryRepository(EcoDbContext context)
        {
            _context = context;
        }

        public async Task AddTransactionAsync(Transactionhistory transaction)
        {
            await _context.Transactionhistories.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTransactionAsync(Transactionhistory transaction)
        {
            _context.Transactionhistories.Update(transaction);
            await _context.SaveChangesAsync();
        }

        // ✅ FIX HIỆU NĂNG: Tìm trực tiếp trong DB, không load hết về RAM
        public async Task<Transactionhistory?> GetByOrderCodeAsync(string orderCode)
        {
            return await _context.Transactionhistories
                .FirstOrDefaultAsync(t => t.OrderCode == orderCode && t.DeletedAt == null);
        }

        public async Task<IEnumerable<Transactionhistory>> GetSuccessTransactionsAsync()
        {
            return await _context.Transactionhistories
                .AsNoTracking()
                .Where(t =>
                    // 1. Xử lý Status: Chuyển về chữ hoa và cắt khoảng trắng thừa
                    t.Status.ToUpper().Trim() == "PAID"

                    // 2. Đảm bảo chưa bị xóa
                    && t.DeletedAt == null)
                .OrderByDescending(t => t.DateTrade) 
                .ToListAsync();
        }

        // Hàm tính tổng tiền theo khoảng thời gian (Trả về số double luôn, không trả List)
        public async Task<double> GetTotalRevenueAsync(DateTime from, DateTime to)
        {
            return await _context.Transactionhistories
                .AsNoTracking()
                .Where(t => t.DeletedAt == null
                            && t.Status.ToUpper() == "PAID" // Tận dụng index nếu có
                            && t.DateTrade >= from
                            && t.DateTrade <= to)
                .SumAsync(t => t.Amount ?? 0);
        }

        // Hàm lấy dữ liệu vẽ biểu đồ (Group By Database - Siêu nhanh)
        public async Task<List<ChartDataPoint>> GetRevenueChartAsync(DateTime from, DateTime to)
        {
            // Logic: Nếu khoảng cách > 31 ngày -> Group theo Tháng. Ngược lại Group theo Ngày.
            var daysDiff = (to - from).TotalDays;
            var isMonthly = daysDiff > 31;

            var query = _context.Transactionhistories
                .AsNoTracking()
                .Where(t => t.DeletedAt == null
                            && t.Status.ToUpper() == "PAID"
                            && t.DateTrade >= from
                            && t.DateTrade <= to);

            if (isMonthly)
            {
                // Group theo Tháng (Postgres)
                return await query
                    .GroupBy(t => new { t.DateTrade.Value.Year, t.DateTrade.Value.Month })
                    .Select(g => new ChartDataPoint
                    {
                        Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                        Value = g.Sum(t => t.Amount ?? 0)
                    })
                    .ToListAsync();
            }
            else
            {
                // Group theo Ngày
                return await query
                    .GroupBy(t => t.DateTrade.Value.Date)
                    .Select(g => new ChartDataPoint
                    {
                        Date = g.Key,
                        Value = g.Sum(t => t.Amount ?? 0)
                    })
                    .ToListAsync();
            }
        }
    }
}
