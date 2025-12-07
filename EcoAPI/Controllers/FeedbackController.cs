using EcoBO.DTO.Feedback;
using EcoBO.DTO.Feedback;
using EcoService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcoAPI.Controllers
{
    [Route("api/feedbacks")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FeedbackFilterParam filter)
        {
            var result = await _feedbackService.GetAllAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var feedback = await _feedbackService.GetByIdAsync(id);
            if (feedback == null) return NotFound();
            return Ok(feedback);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FeedbackRequest request)
        {
            await _feedbackService.AddAsync(request);
            return Ok(new { message = "Feedback created successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FeedbackRequest request)
        {
            await _feedbackService.UpdateAsync(id, request);
            return Ok(new { message = "Feedback updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _feedbackService.DeleteAsync(id);
            return Ok(new { message = "Feedback deleted successfully" });
        }
    }
}
