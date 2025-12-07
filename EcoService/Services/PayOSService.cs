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
using EcoRepository.Interfaces;
using EcoBO.DTO.Donation;
using EcoService.Helpers;
using EcoBO.Settings;
using EcoBO.DTO.PayOS;


namespace EcoService.Services
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOS _client;
        private readonly ITransactionHistoryRepository _transactionRepo;
        private readonly string _checksumKey;

        public PayOSService(IOptions<PayOSSettings> options, ITransactionHistoryRepository transactionRepo)
        {
            var settings = options.Value;
            _client = new PayOS(settings.ClientId, settings.ApiKey, settings.ChecksumKey);
            _transactionRepo = transactionRepo;
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
                Status = "PENDING",
                OrderCode = orderCode.ToString(),
                Description = description ?? "Donation",
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepo.AddTransactionAsync(transaction);

            return result;
        }

        public async Task<PaymentLinkInformation> GetPaymentInfoAsync(long oderCode)
        {
            return await _client.getPaymentLinkInformation((oderCode));
        }


        public async Task<IEnumerable<DonationHistoryDto>> GetPublicDonationHistoryAsync()
        {
            // 1. Chỉ lấy những đơn ĐÃ THANH TOÁN
            var transactions = await _transactionRepo.GetSuccessTransactionsAsync();

            // 2. Chuyển đổi sang DTO
            return transactions.Select(t => new DonationHistoryDto
            {
                Amount = t.Amount,
                Description = t.Description,
                OrderCode = t.OrderCode,
                PaidAt = t.DateTrade.ToVietnamTime()
            }).ToList();
        }

     
        // 🔥 XỬ LÝ WEBHOOK (Phiên bản Clean & Chuẩn)
        public async Task VerifyAndProcessWebhook(WebhookType webhookBody)
        {
            try
            {
                // 1. Xác thực chữ ký
                _client.verifyPaymentWebhookData(webhookBody);

                var data = webhookBody.data;
                string orderCodeStr = data.orderCode.ToString();

                // 2. Tìm đơn hàng trong DB
                var transaction = await _transactionRepo.GetByOrderCodeAsync(orderCodeStr);

                // Nếu không tìm thấy hoặc đơn đã thanh toán xong thì dừng
                if (transaction == null || transaction.Status == "PAID")
                    return;

                // 3. Xử lý cập nhật trạng thái PAID
                // Code "00" đại diện cho giao dịch thành công
                if (data.code == "00")
                {
                    transaction.Status = "PAID";
                    transaction.Amount = data.amount; // Cập nhật số tiền thực nhận
                    transaction.DateTrade = DateTime.UtcNow; // Lưu thời gian thanh toán


                    transaction.UpdatedAt = DateTime.UtcNow;
                    await _transactionRepo.UpdateTransactionAsync(transaction);
                }

            }
            catch (Exception ex)
            {
   
                Console.WriteLine($"Webhook Error: {ex.Message}");
            }
        }
    }

}

