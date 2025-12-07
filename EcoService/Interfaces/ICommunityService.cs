using EcoBO.Common;
using EcoBO.DTO.Community.Request;
using EcoBO.DTO.Community.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoService.Interfaces
{
    public interface ICommunityService
    {
        Task<PagedResult<PostDto>> GetPostsAsync(PostFilterRequest filter, Guid? currentUserId);
        Task CreatePostAsync(Guid userId, CreatePostRequest request);
        Task ToggleLikeAsync(Guid userId, Guid postId);
        Task<PostDto> GetPostDetailBySlugAsync(string slug, Guid? currentUserId);
        Task<List<HashtagDto>> SearchHashtagsAsync(string keyword);
        Task<List<HashtagDto>> GetTrendingHashtagsAsync();
        Task<PagedResult<CommentDto>> GetCommentsAsync(Guid postId, int page, int size);
        Task CreateCommentAsync(Guid userId, CreateCommentRequest request);
    }
}
