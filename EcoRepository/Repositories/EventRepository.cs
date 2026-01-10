using EcoBO.Common;
using EcoBO.Models;
using EcoRepository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcoRepository.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EcoDbContext _context;

        public EventRepository(EcoDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<CommunityEvent>> GetEventsAsync(string keyword, string status, int page, int size)
        {
            var query = _context.CommunityEvents.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(e => e.Status == status);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(e => e.Title.Contains(keyword) || e.Location.Contains(keyword));
            }

            // Sắp xếp: Sự kiện sắp diễn ra lên đầu
            query = query.OrderBy(e => e.StartDate);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            return new PagedResult<CommunityEvent>(items, total, page, size);
        }

        public async Task<CommunityEvent> GetEventByIdAsync(Guid id)
            => await _context.CommunityEvents.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<CommunityEvent> GetEventBySlugAsync(string slug)
            => await _context.CommunityEvents.FirstOrDefaultAsync(e => e.Slug == slug);

        public async Task AddEventAsync(CommunityEvent evt)
        {
            await _context.CommunityEvents.AddAsync(evt);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEventAsync(CommunityEvent evt)
        {
            _context.CommunityEvents.Update(evt);
            await _context.SaveChangesAsync();
        }

        public async Task AddRegistrationAsync(EventRegistration reg)
        {
            await _context.EventRegistrations.AddAsync(reg);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserRegisteredAsync(Guid eventId, Guid userId)
            => await _context.EventRegistrations.AnyAsync(r => r.EventId == eventId && r.UserId == userId);

        public async Task<bool> IsEmailRegisteredAsync(Guid eventId, string email)
            => await _context.EventRegistrations.AnyAsync(r => r.EventId == eventId && r.ParticipantEmail == email);
    }
}