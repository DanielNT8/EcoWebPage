using EcoBO.Common;
using EcoBO.DTO.Community.Request;
using EcoBO.DTO.Community.Response;
using EcoBO.Models;
using EcoRepository.Interfaces;
using EcoService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EcoService.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly ICommunityRepository _repo;

        public CommunityService(ICommunityRepository repo)
        {
            _repo = repo;
        }

        // --- 1. POSTS ---
        public async Task<PagedResult<PostDto>> GetPostsAsync(PostFilterRequest filter, Guid? currentUserId)
        {
            var pagedEntities = await _repo.GetPostsAsync(filter);
            var dtos = new List<PostDto>();

            foreach (var entity in pagedEntities.Items)
            {
                // Lấy Hashtags (đã bỏ dấu # trong DB)
                var hashtags = await _repo.GetHashtagsByPostIdAsync(entity.Id);
                // Frontend tự thêm # khi hiển thị: "#" + tag

                var dto = new PostDto
                {
                    Id = entity.Id,
                    Title = entity.Title,
                    Slug = entity.Slug,
                    Content = entity.Content,
                    ThumbnailUrl = entity.ThumbnailUrl,
                    MediaUrls = entity.MediaUrls ?? new List<string>(),
                    Hashtags = hashtags,

                    ViewCount = entity.ViewCount ?? 0,
                    LikeCount = entity.LikeCount ?? 0,
                    CommentCount = entity.CommentCount ?? 0,
                    CreatedAt = entity.CreatedAt ?? DateTime.UtcNow,

                    AuthorId = entity.UserId,
                    AuthorName = entity.User?.Username ?? "Unknown",
                    AuthorAvatar = entity.User?.AvatarUrl,
                };

                if (currentUserId.HasValue)
                {
                    var interaction = await _repo.GetInteractionAsync(currentUserId.Value, entity.Id, "POST");
                    dto.IsLikedByCurrentUser = interaction != null;
                }

                dtos.Add(dto);
            }

            return new PagedResult<PostDto>(dtos, pagedEntities.TotalCount, pagedEntities.PageIndex, pagedEntities.PageSize);
        }

        public async Task<PostDto> GetPostDetailBySlugAsync(string slug, Guid? currentUserId)
        {
            var entity = await _repo.GetPostBySlugAsync(slug);
            if (entity == null) throw new Exception("Bài viết không tồn tại");

            // Tăng view
            entity.ViewCount = (entity.ViewCount ?? 0) + 1;
            await _repo.UpdatePostAsync(entity);

            var hashtags = await _repo.GetHashtagsByPostIdAsync(entity.Id);

            var dto = new PostDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Slug = entity.Slug,
                Content = entity.Content,
                ThumbnailUrl = entity.ThumbnailUrl,
                MediaUrls = entity.MediaUrls ?? new List<string>(),
                Hashtags = hashtags,

                ViewCount = entity.ViewCount ?? 0,
                LikeCount = entity.LikeCount ?? 0,
                CommentCount = entity.CommentCount ?? 0,
                CreatedAt = entity.CreatedAt ?? DateTime.UtcNow,

                AuthorId = entity.UserId,
                AuthorName = entity.User?.Username ?? "Unknown",
                AuthorAvatar = entity.User?.AvatarUrl,
            };

            if (currentUserId.HasValue)
            {
                var interaction = await _repo.GetInteractionAsync(currentUserId.Value, entity.Id, "POST");
                dto.IsLikedByCurrentUser = interaction != null;
            }

            return dto;
        }

        public async Task CreatePostAsync(Guid userId, CreatePostRequest request)
        {
            var slug = GenerateSlug(request.Title);

            var post = new CommunityPost
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = request.Title,
                Slug = slug,
                Content = request.Content,
                MediaUrls = request.MediaUrls ?? new List<string>(),
                ThumbnailUrl = request.ThumbnailUrl,
                Status = "PUBLISHED",
                CreatedAt = DateTime.UtcNow,
                ViewCount = 0,
                LikeCount = 0,
                CommentCount = 0
            };

            await _repo.AddPostAsync(post);

            // Xử lý Hashtag (Clean & Save)
            if (request.Hashtags != null && request.Hashtags.Any())
            {
                foreach (var tagText in request.Hashtags)
                {
                    // Chuẩn hóa: lowercase, bỏ dấu #
                    string cleanTag = tagText.Trim().ToLower();
                    if (cleanTag.StartsWith("#")) cleanTag = cleanTag.Substring(1);

                    if (string.IsNullOrEmpty(cleanTag)) continue;

                    var existingTag = await _repo.GetHashtagByNameAsync(cleanTag);
                    Guid tagId;

                    if (existingTag == null)
                    {
                        var newTag = new CommunityHashtag
                        {
                            Id = Guid.NewGuid(),
                            TagName = cleanTag, // Lưu không dấu #
                            UsageCount = 1,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _repo.AddHashtagAsync(newTag);
                        tagId = newTag.Id;
                    }
                    else
                    {
                        tagId = existingTag.Id;
                        // TODO: Update usage count
                    }

                    await _repo.AddPostTagAsync(new CommunityPostTag
                    {
                        PostId = post.Id,
                        HashtagId = tagId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        // --- 2. HASHTAGS ---
        public async Task<List<HashtagDto>> SearchHashtagsAsync(string keyword)
        {
            // Chuẩn hóa input search
            keyword = keyword.Trim().ToLower();
            if (keyword.StartsWith("#")) keyword = keyword.Substring(1);

            var tags = await _repo.SearchHashtagsAsync(keyword, 10);
            return tags.Select(t => new HashtagDto
            {
                Id = t.Id,
                TagName = t.TagName,
                UsageCount = t.UsageCount ?? 0
            }).ToList();
        }

        public async Task<List<HashtagDto>> GetTrendingHashtagsAsync()
        {
            var tags = await _repo.GetTrendingHashtagsAsync(5);
            return tags.Select(t => new HashtagDto
            {
                Id = t.Id,
                TagName = t.TagName,
                UsageCount = t.UsageCount ?? 0
            }).ToList();
        }

        // --- 3. COMMENTS ---
        public async Task<PagedResult<CommentDto>> GetCommentsAsync(Guid postId, int page, int size)
        {
            // Lấy danh sách comment phẳng từ DB (đã bao gồm thông tin User)
            var result = await _repo.GetCommentsByPostIdAsync(postId, page, size);

            var dtos = result.Items.Select(c => new CommentDto
            {
                Id = c.Id,
                PostId = c.PostId,
                Content = c.Content,
                MediaUrl = c.MediaUrl,
                CreatedAt = c.CreatedAt ?? DateTime.UtcNow,
                UserId = c.UserId,
                UserName = c.User?.Username ?? "Unknown",
                UserAvatar = c.User?.AvatarUrl,
                ParentCommentId = c.ParentCommentId
                // Replies: Frontend sẽ tự filter hoặc gọi API riêng nếu cần
            }).ToList();

            return new PagedResult<CommentDto>(dtos, result.TotalCount, result.PageIndex, result.PageSize);
        }

        public async Task CreateCommentAsync(Guid userId, CreateCommentRequest request)
        {
            var comment = new CommunityComment
            {
                Id = Guid.NewGuid(),
                PostId = request.PostId,
                UserId = userId,
                Content = request.Content,
                MediaUrl = request.MediaUrl,
                ParentCommentId = request.ParentCommentId,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddCommentAsync(comment);

            // Tăng Comment Count
            var post = await _repo.GetPostByIdAsync(request.PostId);
            if (post != null)
            {
                post.CommentCount = (post.CommentCount ?? 0) + 1;
                await _repo.UpdatePostAsync(post);
            }
        }

        // --- 4. INTERACTIONS ---
        public async Task ToggleLikeAsync(Guid userId, Guid postId)
        {
            var post = await _repo.GetPostByIdAsync(postId);
            if (post == null) throw new Exception("Post not found");

            var existingLike = await _repo.GetInteractionAsync(userId, postId, "POST");

            if (existingLike != null)
            {
                await _repo.RemoveInteractionAsync(existingLike);
                post.LikeCount = Math.Max(0, (post.LikeCount ?? 0) - 1);
            }
            else
            {
                var interaction = new CommunityInteraction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TargetId = postId,
                    TargetType = "POST",
                    InteractionType = "LIKE",
                    CreatedAt = DateTime.UtcNow
                };
                await _repo.AddInteractionAsync(interaction);
                post.LikeCount = (post.LikeCount ?? 0) + 1;
            }

            await _repo.UpdatePostAsync(post);
        }

        private string GenerateSlug(string title)
        {
            string slug = title.ToLower();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-").Trim();
            return $"{slug}-{Guid.NewGuid().ToString("N").Substring(0, 6)}";
        }
    }
}
