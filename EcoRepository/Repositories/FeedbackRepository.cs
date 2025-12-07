using EcoBO.Common;
using EcoBO.DTO.Feedback;
using EcoBO.Models;
using EcoRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoRepository.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly EcoDbContext _context;

        public FeedbackRepository(EcoDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Feedback>> GetFeedbacksAsync(FeedbackFilterParam filter)
        {
            // Sanitize / defaults
            if (filter == null) filter = new FeedbackFilterParam();
            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 10 : Math.Min(filter.PageSize, 100); // clamp max page size

            var query = _context.Feedbacks.AsQueryable();

            // Exclude soft-deleted by default (adjust if your app uses other semantics)
            query = query.Where(f => f.DeletedAt == null && (f.Status == null || f.Status != "Deleted"));

            // Search (case-insensitive) - use EF.Functions.ILike for Postgres
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var s = filter.Search.Trim();
                // ILike is translated to SQL ILIKE (Postgres) => case-insensitive pattern match
                query = query.Where(f =>
                    EF.Functions.ILike(f.Message ?? string.Empty, $"%{s}%") ||
                    EF.Functions.ILike(f.ContactInfo ?? string.Empty, $"%{s}%") ||
                    EF.Functions.ILike(f.UserName ?? string.Empty, $"%{s}%"));
            }

            // Filter by Status (case-insensitive equality)
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                var status = filter.Status.Trim();
                // use ILike for case-insensitive equality or compare lowercased values
                query = query.Where(f => EF.Functions.ILike(f.Status ?? string.Empty, status));
            }

            // Sort: normalize SortBy and apply stable ordering (handle nullable CreatedAt)
            var sortBy = (filter.SortBy ?? string.Empty).Trim().ToLowerInvariant();
            var sortAsc = filter.SortAscending;

            query = sortBy switch
            {
                "username" => sortAsc ? query.OrderBy(f => f.UserName) : query.OrderByDescending(f => f.UserName),
                "message" => sortAsc ? query.OrderBy(f => f.Message) : query.OrderByDescending(f => f.Message),
                "contactinfo" => sortAsc ? query.OrderBy(f => f.ContactInfo) : query.OrderByDescending(f => f.ContactInfo),
                // order by CreatedAt with nulls treated as "oldest" for consistent behavior
                _ => sortAsc
                        ? query.OrderBy(f => f.CreatedAt ?? DateTime.MinValue)
                        : query.OrderByDescending(f => f.CreatedAt ?? DateTime.MinValue),
            };

            // Paging: do CountAsync before materializing
            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Feedback>(items, totalItems, pageNumber, pageSize);
        }


        public async Task<Feedback?> GetByIdAsync(Guid id)
            => await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == id);

        public async Task AddAsync(Feedback feedback)
        {
            feedback.Id = Guid.NewGuid();
            feedback.CreatedAt = DateTime.Now;
            feedback.Status = "Active";
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Feedback feedback)
        {
            feedback.UpdatedAt = DateTime.Now;
            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Feedback feedback)
        {
            feedback.Status = "Deleted";
            feedback.DeletedAt = DateTime.Now;
            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
        }
    }
}
