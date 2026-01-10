using EcoBO.Common;
using EcoBO.DTO.Event;

namespace EcoService.Interfaces
{
    public interface IEventService
    {
        Task<PagedResult<EventDto>> GetEventsAsync(string keyword, int page, int size, Guid? currentUserId);
        Task<EventDto> GetEventDetailAsync(string slug, Guid? currentUserId);

        // Admin Only
        Task CreateEventAsync(Guid adminId, CreateEventRequest request);

        // User & Guest
        Task JoinEventAsync(Guid eventId, Guid? userId, JoinEventRequest request);
    }
}