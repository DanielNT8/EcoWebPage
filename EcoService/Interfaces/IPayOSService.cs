using EcoBO.DTO.Donation;
using EcoBO.Models;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoService.Interfaces
{
    public interface IPayOSService
    {
        Task<CreatePaymentResult> CreatePaymentLinkAsync(int amount, string description, string cancelUrl, string returnUrl);
        Task<PaymentLinkInformation> GetPaymentInfoAsync(long paymentLinkId);
        Task<IEnumerable<DonationHistoryDto>> GetPublicDonationHistoryAsync();
        Task VerifyAndProcessWebhook(WebhookType webhookBody);
    }
}
