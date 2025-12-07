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
    }
}
