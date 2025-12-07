using EcoBO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoRepository.Interfaces
{
    public interface ITransactionHistoryRepository
    {
        Task AddTransactionAsync(Transactionhistory transaction);
        Task UpdateTransactionAsync(Transactionhistory transaction);
        Task<Transactionhistory?> GetByOrderCodeAsync(string orderCode);
        Task<IEnumerable<Transactionhistory>> GetSuccessTransactionsAsync();
    }
}
