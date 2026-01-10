using EcoBO.DTO.Event;
using EcoService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcoAPI.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _service;

        public EventController(IEventService service)
        {
            _service = service;
        }

        // 1. Lấy danh sách (PUBLIC - Ai cũng xem được)
        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] string? keyword, [FromQuery] int page = 1)
        {
            // Lấy ID user nếu đang đăng nhập (để check IsRegistered)
            Guid? userId = GetCurrentUserId();
            var result = await _service.GetEventsAsync(keyword, page, 10, userId);
            return Ok(result);
        }

        // 2. Lấy chi tiết (PUBLIC)
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetEventDetail(string slug)
        {
            Guid? userId = GetCurrentUserId();
            try
            {
                var result = await _service.GetEventDetailAsync(slug, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // 3. Tạo sự kiện (ADMIN ONLY - Chỉ Admin mới được tạo)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            try
            {
                var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                await _service.CreateEventAsync(adminId, request);
                return Ok(new { message = "Tạo sự kiện thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 4. Đăng ký tham gia (AUTHENTICATED USER & GUEST)
        // Logic: Ai cũng gọi được, nhưng Service sẽ xử lý khác nhau dựa trên userId
        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinEvent(Guid id, [FromBody] JoinEventRequest request)
        {
            Guid? userId = GetCurrentUserId();

            try
            {
                await _service.JoinEventAsync(id, userId, request);
                return Ok(new { message = "Đăng ký tham gia thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Helper lấy User ID an toàn (trả về null nếu chưa login)
        private Guid? GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null && Guid.TryParse(claim.Value, out Guid id))
                {
                    return id;
                }
            }
            return null;
        }
    }
}