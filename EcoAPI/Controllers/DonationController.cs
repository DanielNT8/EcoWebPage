using EcoBO.DTO.Donation;
using EcoBO.Models;
using EcoRepository.Repositories;
using EcoService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;


namespace EcoAPI.Controllers
{
    [ApiController]
    [Route("api/donations")]
    public class DonationController : ControllerBase
    {
        private readonly IPayOSService _payOSService;
        
        public DonationController(IPayOSService payOSService)
        {
            _payOSService = payOSService;
        }


        [HttpPost]
        public async Task<IActionResult> Donation([FromBody] DonationRequest req)
        {
            if (req.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");
                

            try
            {
                string cancelUrl = "https://www.eco.info.vn/";
                string returnUrl = "https://www.eco.info.vn/";

                var result = await _payOSService.CreatePaymentLinkAsync(
                    req.Amount, req.Description, cancelUrl, returnUrl);

                var response = new DonationResponse
                {
                    PaymentLinkId = result.paymentLinkId,
                    CheckoutUrl = result.checkoutUrl,
                    Description = req.Description ?? "Donation",
                    Status = "PENDING",
                    OrderCode = result.orderCode.ToString() // ✅ Trả về mã đơn hàng
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Payment creation failed", error = ex.Message });
            }
        }

        // 2. Webhook nhận dữ liệu từ PayOS (Tự động chạy ngầm)
        [HttpPost("webhook")]
        public async Task<IActionResult> PayOSWebhook([FromBody] WebhookType webhookBody)
        {
            try
            {
                await _payOSService.VerifyAndProcessWebhook(webhookBody);
                return Ok(new { message = "Webhook processed successfully" });
            }
            catch (Exception ex)
            {
                // Log lỗi ở đây để debug
                Console.WriteLine($"Webhook Error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{oderCode}")]
        public async Task<IActionResult> GetPaymentInfo(long oderCode)
        {
            var info = await _payOSService.GetPaymentInfoAsync(oderCode);
            return Ok(info);
        }



        [HttpGet("history")]
        public async Task<IActionResult> GetPublicDonationHistory()
        {
            try
            {
                // Controller không biết gì về logic map hay entity, chỉ nhận DTO từ Service
                var result = await _payOSService.GetPublicDonationHistoryAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi tải lịch sử ủng hộ", error = ex.Message });
            }
        }

    }
}
