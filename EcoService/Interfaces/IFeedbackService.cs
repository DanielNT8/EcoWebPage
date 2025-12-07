using EcoBO.Common;
using EcoBO.DTO.Feedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoService.Interfaces
{
    public interface IFeedbackService
    {
        Task<PagedResult<FeedbackResponse>> GetAllAsync(FeedbackFilterParam filter);
        Task<FeedbackResponse?> GetByIdAsync(Guid id);
        Task AddAsync(FeedbackRequest request);
        Task UpdateAsync(Guid id, FeedbackRequest request);
        Task DeleteAsync(Guid id);
    }
}
