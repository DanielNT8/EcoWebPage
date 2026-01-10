using EcoBO.Common; // Giả định có class PagedResult
using EcoBO.Models;

namespace EcoRepository.Interfaces
{
    public interface IEventRepository
    {
        // Lấy danh sách (Có phân trang, search)
        Task<PagedResult<CommunityEvent>> GetEventsAsync(string keyword, string status, int page, int size);

        // Lấy chi tiết
        Task<CommunityEvent> GetEventByIdAsync(Guid id);
        Task<CommunityEvent> GetEventBySlugAsync(string slug);

        // CRUD Event
        Task AddEventAsync(CommunityEvent evt);
        Task UpdateEventAsync(CommunityEvent evt);

        // Registration Logic
        Task AddRegistrationAsync(EventRegistration reg);

        // Kiểm tra xem User (theo ID) đã đăng ký event này chưa
        Task<bool> IsUserRegisteredAsync(Guid eventId, Guid userId);

        // Kiểm tra xem Email này đã đăng ký event này chưa (Dùng cho Guest)
        Task<bool> IsEmailRegisteredAsync(Guid eventId, string email);
    }
}