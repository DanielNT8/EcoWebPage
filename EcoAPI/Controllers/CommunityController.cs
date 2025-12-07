using EcoBO.DTO.Community.Request;
using EcoService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcoAPI.Controllers
{
    [Route("api/community")]
    [ApiController]
    public class CommunityController : ControllerBase
    {
        private readonly ICommunityService _service;

        public CommunityController(ICommunityService service)
        {
            _service = service;
        }

        // GET: /api/community/posts?tag=#eco&pageIndex=1
        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts([FromQuery] PostFilterRequest filter)
        {
            Guid? userId = null;
            if (User.Identity.IsAuthenticated)
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null) userId = Guid.Parse(claim.Value);
            }

            var result = await _service.GetPostsAsync(filter, userId);
            return Ok(result);
        }

        // POST: /api/community/posts
        [Authorize]
        [HttpPost("posts")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await _service.CreatePostAsync(userId, request);
            return Ok(new { message = "Post created successfully" });
        }

        // POST: /api/community/posts/{id}/like
        [Authorize]
        [HttpPost("posts/{id}/like")]
        public async Task<IActionResult> ToggleLike(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await _service.ToggleLikeAsync(userId, id);
            return Ok(new { message = "Success" });
        }

        [HttpGet("posts/{slug}")]
        public async Task<IActionResult> GetPostBySlug(string slug)
        {
            Guid? userId = null;
            if (User.Identity.IsAuthenticated)
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null) userId = Guid.Parse(claim.Value);
            }

            var result = await _service.GetPostDetailBySlugAsync(slug, userId);
            return Ok(result);
        }

        // 5. Gợi ý Hashtag (Dùng cho Dropdown Autocomplete)
        // GET: /api/community/hashtags/search?keyword=eco
        [HttpGet("hashtags/search")]
        public async Task<IActionResult> SearchHashtags([FromQuery] string keyword)
        {
            var result = await _service.SearchHashtagsAsync(keyword);
            return Ok(result);
        }

        // 6. Top Trending Hashtags (Dùng cho Sidebar)
        [HttpGet("hashtags/trending")]
        public async Task<IActionResult> GetTrendingHashtags()
        {
            var result = await _service.GetTrendingHashtagsAsync();
            return Ok(result);
        }

        // 7. Lấy danh sách bình luận
        [HttpGet("posts/{postId}/comments")]
        public async Task<IActionResult> GetComments(Guid postId, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _service.GetCommentsAsync(postId, page, size);
            return Ok(result);
        }

        // 8. Đăng bình luận
        [Authorize]
        [HttpPost("comments")]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            await _service.CreateCommentAsync(Guid.Parse(userIdStr), request);
            return Ok(new { message = "Bình luận thành công" });
        }
    }
}
