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
            // 1. AsNoTracking(): BẮT BUỘC khi chỉ đọc dữ liệu để tăng tốc độ truy vấn
            var query = _context.Feedbacks.AsNoTracking();

            // 2. Soft Delete Logic: Chỉ lấy bản ghi chưa bị xóa
            query = query.Where(f => f.DeletedAt == null);

            // 3. Filter Status (Nghiệp vụ)
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                var status = filter.Status.Trim();
                query = query.Where(f => f.Status.ToLower() == status.ToLower());
            }

            // 4. Search Keyword (Sử dụng GIN Index đã tạo ở DB)
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var k = filter.Keyword.Trim();
                // Ghép chuỗi để tìm trên cả 3 trường
                query = query.Where(f => EF.Functions.ILike(
                    (f.Message ?? "") + " " + (f.ContactInfo ?? "") + " " + (f.UserName ?? ""),
                    $"%{k}%"));
            }

            // 5. Sorting (Xử lý Clean Code)
            query = (filter.SortBy?.ToLower(), filter.IsDescending) switch
            {
                ("username", true) => query.OrderByDescending(f => f.UserName),
                ("username", false) => query.OrderBy(f => f.UserName),
                ("message", true) => query.OrderByDescending(f => f.Message),
                ("message", false) => query.OrderBy(f => f.Message),
                // Mặc định sort theo CreatedAt
                (_, false) => query.OrderBy(f => f.CreatedAt),
                _ => query.OrderByDescending(f => f.CreatedAt)
            };

            // 6. Phân trang & Thực thi query
            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<Feedback>(items, totalItems, filter.PageIndex, filter.PageSize);
        }

        public async Task<Feedback?> GetByIdAsync(Guid id)
        {
            // Cũng cần check DeletedAt == null để tránh lôi ra feedback đã xóa
            return await _context.Feedbacks
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id && f.DeletedAt == null);
        }

        public async Task AddAsync(Feedback feedback)
        {

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
            feedback.DeletedAt = DateTime.Now;
            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
        }
    }
}
