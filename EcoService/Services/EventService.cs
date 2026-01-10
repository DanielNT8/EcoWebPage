using EcoBO.Common;
using EcoBO.DTO.Event;
using EcoBO.Models;
using EcoRepository.Interfaces;
using EcoService.Interfaces;
using System.Text.RegularExpressions;

namespace EcoService.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _repo;

        public EventService(IEventRepository repo)
        {
            _repo = repo;
        }

        public async Task CreateEventAsync(Guid adminId, CreateEventRequest request)
        {
            // 1. Validate Business Logic
            if (request.StartDate >= request.EndDate)
                throw new Exception("Ngày kết thúc phải sau ngày bắt đầu.");

            if (request.StartDate < DateTime.UtcNow)
                throw new Exception("Không thể tạo sự kiện trong quá khứ.");

            // 2. Map Entity
            var evt = new CommunityEvent
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Slug = GenerateSlug(request.Title), // Hàm sinh slug từ title
                ThumbnailUrl = request.ThumbnailUrl,
                Description = request.Description,
                Location = request.Location,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                MaxParticipants = request.MaxParticipants,
                CurrentParticipants = 0,
                Status = "PUBLISHED", // Hoặc DRAFT nếu muốn duyệt
                CreatedBy = adminId,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddEventAsync(evt);
        }

        public async Task JoinEventAsync(Guid eventId, Guid? userId, JoinEventRequest request)
        {
            var evt = await _repo.GetEventByIdAsync(eventId);
            if (evt == null) throw new Exception("Sự kiện không tồn tại.");

            // 1. Kiểm tra điều kiện sự kiện
            if (evt.Status != "PUBLISHED") throw new Exception("Sự kiện chưa mở đăng ký.");
            if (evt.EndDate < DateTime.UtcNow) throw new Exception("Sự kiện đã kết thúc.");
            if (evt.MaxParticipants.HasValue && evt.CurrentParticipants >= evt.MaxParticipants.Value)
                throw new Exception("Sự kiện đã đủ số lượng người tham gia.");

            // 2. Kiểm tra Trùng lặp (Tránh spam)
            if (userId.HasValue)
            {
                // Nếu là User đăng nhập -> Check theo ID
                if (await _repo.IsUserRegisteredAsync(eventId, userId.Value))
                    throw new Exception("Bạn đã đăng ký tham gia sự kiện này rồi.");
            }
            else
            {
                // Nếu là Guest -> Check theo Email
                if (await _repo.IsEmailRegisteredAsync(eventId, request.ParticipantEmail))
                    throw new Exception("Email này đã được sử dụng để đăng ký sự kiện này.");
            }

            // 3. Tạo đăng ký
            var reg = new EventRegistration
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                UserId = userId, // Null nếu là Guest

                // Snapshot thông tin (Quan trọng: Luôn lưu lại để User có thể sửa đổi cho riêng sự kiện này)
                ParticipantName = request.ParticipantName,
                ParticipantEmail = request.ParticipantEmail,
                ParticipantPhone = request.ParticipantPhone,

                Status = "REGISTERED",
                RegisteredAt = DateTime.UtcNow
            };

            await _repo.AddRegistrationAsync(reg);

            // 4. Update số lượng người tham gia
            evt.CurrentParticipants++;
            await _repo.UpdateEventAsync(evt);
        }

        public async Task<PagedResult<EventDto>> GetEventsAsync(string keyword, int page, int size, Guid? currentUserId)
        {
            var result = await _repo.GetEventsAsync(keyword, "PUBLISHED", page, size);

            var dtos = new List<EventDto>();
            foreach (var item in result.Items)
            {
                var dto = MapToDto(item);

                // Nếu user đang đăng nhập, check xem đã tham gia chưa để hiện UI phù hợp
                if (currentUserId.HasValue)
                {
                    dto.IsRegistered = await _repo.IsUserRegisteredAsync(item.Id, currentUserId.Value);
                }
                dtos.Add(dto);
            }

            return new PagedResult<EventDto>(dtos, result.TotalCount, result.PageIndex, result.PageSize);
        }

        public async Task<EventDto> GetEventDetailAsync(string slug, Guid? currentUserId)
        {
            var item = await _repo.GetEventBySlugAsync(slug);
            if (item == null) throw new Exception("Không tìm thấy sự kiện");

            var dto = MapToDto(item);
            if (currentUserId.HasValue)
            {
                dto.IsRegistered = await _repo.IsUserRegisteredAsync(item.Id, currentUserId.Value);
            }
            return dto;
        }

        // Helper Methods
        private EventDto MapToDto(CommunityEvent e)
        {
            return new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Slug = e.Slug,
                ThumbnailUrl = e.ThumbnailUrl,
                Description = e.Description,
                Location = e.Location,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                MaxParticipants = e.MaxParticipants,
                CurrentParticipants = (int)e.CurrentParticipants,
                Status = e.Status
            };
        }

        private string GenerateSlug(string title)
        {
            string slug = title.ToLower();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-").Trim();
            return $"{slug}-{Guid.NewGuid().ToString("N").Substring(0, 6)}";
        }
    }
}