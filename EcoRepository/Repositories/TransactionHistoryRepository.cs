using EcoBO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoRepository.Repositories
{
    public class TransactionHistoryRepository
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

        public async Task<Transactionhistory?> GetByIdAsync(Guid id)
        {
            return await _context.Transactionhistories.FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null);
        }

        public async Task<IEnumerable<Transactionhistory>> GetAllTransactionsAsync()
        {
            return await _context.Transactionhistories
                .OrderByDescending(t => t.DateTrade)
                .ToListAsync();
        }
    }
}
