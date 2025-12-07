using EcoBO.Common;
using EcoBO.DTO.Feedback;
using EcoBO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoRepository.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<PagedResult<Feedback>> GetFeedbacksAsync(FeedbackFilterParam filter);
        Task<Feedback?> GetByIdAsync(Guid feedbackId);
        Task AddAsync(Feedback feedback);
        Task UpdateAsync(Feedback feedback);
        Task DeleteAsync(Feedback feedback);

    }
}
