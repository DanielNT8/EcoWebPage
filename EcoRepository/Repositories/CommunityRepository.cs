using EcoBO.Common;
using EcoBO.DTO.Community.Request;
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
    public class CommunityRepository : ICommunityRepository
    {
        private readonly EcoDbContext _context;

        public CommunityRepository(EcoDbContext context)
        {
            _context = context;
        }

        // --- POSTS ---
        public async Task<PagedResult<CommunityPost>> GetPostsAsync(PostFilterRequest filter)
        {
            var query = _context.CommunityPosts
                .Include(p => p.User)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(p => p.Status == filter.Status);

            if (filter.UserId.HasValue)
                query = query.Where(p => p.UserId == filter.UserId);

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                string kw = filter.Keyword.ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(kw) || p.Content.ToLower().Contains(kw));
            }

            if (!string.IsNullOrEmpty(filter.Tag))
            {
                // Tìm kiếm theo tag (đã bỏ dấu #)
                string tagSearch = filter.Tag.Trim().ToLower();
                if (tagSearch.StartsWith("#")) tagSearch = tagSearch.Substring(1);

                query = query.Where(p => p.CommunityPostTags.Any(pt => pt.Hashtag.TagName == tagSearch));
            }

            query = filter.IsDescending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt);

            int totalCount = await query.CountAsync();
            var items = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<CommunityPost>(items, totalCount, filter.PageIndex, filter.PageSize);
        }

        public async Task<CommunityPost?> GetPostByIdAsync(Guid id)
        {
            return await _context.CommunityPosts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<CommunityPost?> GetPostBySlugAsync(string slug)
        {
            return await _context.CommunityPosts
                .Include(p => p.User)
                // Include Tag để hiển thị chi tiết bài viết
                .Include(p => p.CommunityPostTags).ThenInclude(pt => pt.Hashtag)
                .FirstOrDefaultAsync(p => p.Slug == slug);
        }

        public async Task AddPostAsync(CommunityPost post)
        {
            await _context.CommunityPosts.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePostAsync(CommunityPost post)
        {
            _context.CommunityPosts.Update(post);
            await _context.SaveChangesAsync();
        }

        // --- HASHTAGS ---
        public async Task<CommunityHashtag?> GetHashtagByNameAsync(string tagName)
        {
            return await _context.CommunityHashtags.FirstOrDefaultAsync(h => h.TagName == tagName);
        }

        public async Task AddHashtagAsync(CommunityHashtag hashtag)
        {
            await _context.CommunityHashtags.AddAsync(hashtag);
            await _context.SaveChangesAsync();
        }

        public async Task AddPostTagAsync(CommunityPostTag postTag)
        {
            await _context.CommunityPostTags.AddAsync(postTag);
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetHashtagsByPostIdAsync(Guid postId)
        {
            return await _context.CommunityPostTags
                .Where(pt => pt.PostId == postId)
                .Select(pt => pt.Hashtag.TagName)
                .ToListAsync();
        }

        public async Task<List<CommunityHashtag>> SearchHashtagsAsync(string keyword, int count)
        {
            // Keyword đã được xử lý bỏ # ở Service
            return await _context.CommunityHashtags
                .Where(h => h.TagName.ToLower().Contains(keyword))
                .OrderByDescending(h => h.UsageCount)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<CommunityHashtag>> GetTrendingHashtagsAsync(int count)
        {
            return await _context.CommunityHashtags
                .OrderByDescending(h => h.UsageCount)
                .Take(count)
                .ToListAsync();
        }

        // --- INTERACTIONS ---
        public async Task<CommunityInteraction?> GetInteractionAsync(Guid userId, Guid targetId, string type)
        {
            return await _context.CommunityInteractions
                .FirstOrDefaultAsync(x => x.UserId == userId && x.TargetId == targetId && x.TargetType == type);
        }

        public async Task AddInteractionAsync(CommunityInteraction interaction)
        {
            await _context.CommunityInteractions.AddAsync(interaction);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveInteractionAsync(CommunityInteraction interaction)
        {
            _context.CommunityInteractions.Remove(interaction);
            await _context.SaveChangesAsync();
        }

        // --- COMMENTS ---
        public async Task<PagedResult<CommunityComment>> GetCommentsByPostIdAsync(Guid postId, int pageIndex, int pageSize)
        {
            var query = _context.CommunityComments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .OrderBy(c => c.CreatedAt);

            int total = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<CommunityComment>(items, total, pageIndex, pageSize);
        }

        public async Task AddCommentAsync(CommunityComment comment)
        {
            await _context.CommunityComments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }
    }
}
