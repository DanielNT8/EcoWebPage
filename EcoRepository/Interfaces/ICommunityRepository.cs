using EcoBO.Common;
using EcoBO.DTO.Community.Request;
using EcoBO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoRepository.Interfaces
{
    public interface ICommunityRepository
    {
        // Posts
        Task<PagedResult<CommunityPost>> GetPostsAsync(PostFilterRequest filter);
        Task<CommunityPost?> GetPostByIdAsync(Guid id);
        Task AddPostAsync(CommunityPost post);
        Task UpdatePostAsync(CommunityPost post);
        Task<CommunityPost?> GetPostBySlugAsync(string slug);

        // Interactions
        Task<CommunityInteraction?> GetInteractionAsync(Guid userId, Guid targetId, string type);
        Task AddInteractionAsync(CommunityInteraction interaction);
        Task RemoveInteractionAsync(CommunityInteraction interaction);

        // Hashtags
        Task<CommunityHashtag?> GetHashtagByNameAsync(string tagName);
        Task AddHashtagAsync(CommunityHashtag hashtag);
        Task AddPostTagAsync(CommunityPostTag postTag);
        Task<List<CommunityHashtag>> SearchHashtagsAsync(string keyword, int count); // Mới thêm: Tìm kiếm
        Task<List<CommunityHashtag>> GetTrendingHashtagsAsync(int count);
        Task<List<string>> GetHashtagsByPostIdAsync(Guid postId);

        // --- COMMENT ---
        // Lấy comment của 1 bài viết (kèm phân trang)
        Task<PagedResult<CommunityComment>> GetCommentsByPostIdAsync(Guid postId, int pageIndex, int pageSize);
        Task AddCommentAsync(CommunityComment comment);
    }
}
