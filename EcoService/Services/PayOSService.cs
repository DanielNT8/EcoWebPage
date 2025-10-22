using EcoBO.DTO.PayOS;
using EcoService.Interfaces;
using Microsoft.Extensions.Options;
using Net.payOS.Types;
using Net.payOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoBO.Models;
using EcoRepository.Repositories;


namespace EcoService.Services
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOS _client;
        private readonly TransactionHistoryRepository _transactionRepo;
        private readonly string _checksumKey;

        public PayOSService(IOptions<PayOSSettings> options, TransactionHistoryRepository transactionRepo)
        {
            var settings = options.Value;
            _client = new PayOS(settings.ClientId, settings.ApiKey, settings.ChecksumKey, settings.PartnerCode);
            _transactionRepo = transactionRepo;
            _checksumKey = settings.ChecksumKey;
        }

        public async Task<CreatePaymentResult> CreatePaymentLinkAsync(int amount, string description, string cancelUrl, string returnUrl)
        {
            long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var items = new List<ItemData>
            {
                new ItemData("Donation", 1, amount)
            };

            var paymentData = new PaymentData(orderCode, amount, description ?? "Donation", items, cancelUrl, returnUrl);
            var result = await _client.createPaymentLink(paymentData);

            // Lưu lịch sử giao dịch vào DB
            var transaction = new Transactionhistory
            {
                Id = Guid.Parse(result.paymentLinkId),
                Amount = amount,
                DateTrade = DateTime.Now,
                Status = "Pending",
                OrderCode = orderCode.ToString(),
                Description = description ?? "Donation",
                CreatedAt = DateTime.Now
            };

            await _transactionRepo.AddTransactionAsync(transaction);

            return result;
        }

        public async Task<PaymentLinkInformation> GetPaymentInfoAsync(long oderCode)
        {
            return await _client.getPaymentLinkInformation((oderCode));
        }


        public async Task<IEnumerable<Transactionhistory>> GetAllTransactionsAsync()
        {
            return await _transactionRepo.GetAllTransactionsAsync();
        }


        // 🧠 Cập nhật trạng thái giao dịch
        public async Task<Transactionhistory> UpdateTransactionStatusAsync(string orderCode, string status)
        {
            var transactions = await _transactionRepo.GetAllTransactionsAsync();
            var transaction = transactions.FirstOrDefault(t => t.OrderCode == orderCode);

            if (transaction == null)
                return null;

            transaction.Status = status.ToUpper() switch
            {
                "PAID" or "COMPLETED" => "Paid",
                "CANCELLED" or "FAILED" => "Cancelled",
                _ => "Pending"
            };

            transaction.UpdatedAt = DateTime.Now;
            await _transactionRepo.UpdateTransactionAsync(transaction);

            return transaction;
        }
    }

}

