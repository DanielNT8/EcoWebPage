using EcoBO.Common;
using EcoBO.DTO.Feedback;
using EcoBO.Models;
using EcoRepository.Interfaces;
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
            var pagedFeedbacks = await _repo.GetFeedbacksAsync(filter);

            var responses = pagedFeedbacks.Items.Select(f => new FeedbackResponse
            {
                Id = f.Id,
                UserName = f.UserName,
                Message = f.Message,
                ContactInfo = f.ContactInfo,
                CreatedAt = f.CreatedAt ?? DateTime.Now,
            }).ToList();

            return new PagedResult<FeedbackResponse>(responses, pagedFeedbacks.TotalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<FeedbackResponse?> GetByIdAsync(Guid id)
        {
            var feedback = await _repo.GetByIdAsync(id);
            if (feedback == null) return null;

            return new FeedbackResponse
            {
                Id = feedback.Id,
                UserName = feedback.UserName,
                Message = feedback.Message,
                ContactInfo = feedback.ContactInfo,
                CreatedAt = feedback.CreatedAt ?? DateTime.Now,
            };
        }

        public async Task AddAsync(FeedbackRequest request)
        {
            var feedback = new Feedback
            {
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
