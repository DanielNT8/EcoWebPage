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
                string cancelUrl = "https://your-frontend.com/payment-cancel";
                string returnUrl = "https://www.eco.info.vn/";

                var result = await _payOSService.CreatePaymentLinkAsync(
                    req.Amount, req.Description, cancelUrl, returnUrl);

                var response = new DonationResponse
                {
                    PaymentLinkId = result.paymentLinkId,
                    CheckoutUrl = result.checkoutUrl,
                    Description = req.Description ?? "Donation",
                    Status = "Pending",
                    OrderCode = result.orderCode.ToString() // ✅ Trả về mã đơn hàng
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Payment creation failed", error = ex.Message });
            }
        }


        [HttpGet("{oderCode}")]
        public async Task<IActionResult> GetPaymentInfo(long oderCode)
        {
            var info = await _payOSService.GetPaymentInfoAsync(oderCode);
            return Ok(info);
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetAllTransactions()
        {
            try
            {
                var transactions = await _payOSService.GetAllTransactionsAsync();

                // (Tùy chọn) map sang DTO nếu bạn không muốn trả full entity
                var response = transactions.Select(t => new
                {
                    t.Id,
                    t.OrderCode,
                    t.Amount,
                    t.Status,
                    t.DateTrade,
                    t.CreatedAt
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch transactions", error = ex.Message });
            }
        }

    }
}
