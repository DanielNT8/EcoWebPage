using EcoBO.Common;
using EcoBO.DTO.Feedback;
using EcoBO.Models;
using EcoRepository.Interfaces;
using EcoService.Helpers;
using EcoService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoService.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _repo;

        public FeedbackService(IFeedbackRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<FeedbackResponse>> GetAllAsync(FeedbackFilterParam filter)
        {
            // 1. Gọi Repo lấy Entity
            var pagedEntities = await _repo.GetFeedbacksAsync(filter);

            // 2. Mapping Entity -> DTO (Thực hiện tại Service)
            // Việc loop ở đây chấp nhận được với PageSize nhỏ (10-20-50 item)
            var responseItems = pagedEntities.Items.Select(f => new FeedbackResponse
            {
                Id = f.Id,
                UserName = f.UserName,
                Message = f.Message,
                ContactInfo = f.ContactInfo,
                // Xử lý Timezone hiển thị tại đây
                CreatedAt = f.CreatedAt ?? DateTime.UtcNow
            }).ToList();

            // 3. Trả về PagedResult mới chứa DTO
            return new PagedResult<FeedbackResponse>(
                responseItems,
                pagedEntities.TotalCount,
                filter.PageIndex,
                filter.PageSize
            );
        }

       public async Task<FeedbackResponse?> GetByIdAsync(Guid id)
        {
            var f = await _repo.GetByIdAsync(id);
            if (f == null) return null;

            return new FeedbackResponse
            {
                Id = f.Id,
                UserName = f.UserName,
                Message = f.Message,
                ContactInfo = f.ContactInfo,
                CreatedAt = f.CreatedAt ?? DateTime.UtcNow
            };
        }

        public async Task AddAsync(FeedbackRequest request)
        {
            var feedback = new Feedback
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Message = request.Message ?? string.Empty,
                ContactInfo = request.ContactInfo ?? string.Empty,
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            await _repo.AddAsync(feedback);
        }

        public async Task UpdateAsync(Guid id, FeedbackRequest request)
        {
            var feedback = await _repo.GetByIdAsync(id);
            if (feedback == null) throw new Exception("Feedback not found");

            feedback.UserName = request.UserName;
            feedback.Message = request.Message;
            feedback.ContactInfo = request.ContactInfo;

            await _repo.UpdateAsync(feedback);
        }

        public async Task DeleteAsync(Guid id)
        {
            var feedback = await _repo.GetByIdAsync(id);
            if (feedback == null) throw new Exception("Feedback not found");

            await _repo.DeleteAsync(feedback);
        }
    }
}
