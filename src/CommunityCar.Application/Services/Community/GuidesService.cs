using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Guides.DTOs;
using CommunityCar.Application.Features.Guides.ViewModels;
using CommunityCar.Domain.Entities.Auth;
using CommunityCar.Domain.Entities.Community.Guides;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CommunityCar.Application.Services.Community;

public class GuidesService : IGuidesService
{
    private readonly IGuidesRepository _guidesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly IGuidesNotificationService _notificationService;

    public GuidesService(
        IGuidesRepository guidesRepository,
        IUnitOfWork unitOfWork,
        UserManager<User> userManager,
        IGuidesNotificationService notificationService)
    {
        _guidesRepository = guidesRepository;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public async Task<GuideDetailVM?> GetGuideAsync(Guid id, Guid? currentUserId = null)
    {
        var guide = await _guidesRepository.GetGuideWithAuthorAsync(id);
        if (guide == null) return null;

        var guideVM = await MapToGuideVMAsync(guide, currentUserId);
        var relatedGuides = await _guidesRepository.GetRelatedGuidesAsync(id, 5);
        var authorOtherGuides = await _guidesRepository.GetGuidesByAuthorAsync(guide.AuthorId, 5);

        var relatedGuideVMs = new List<GuideVM>();
        foreach (var relatedGuide in relatedGuides)
        {
            relatedGuideVMs.Add(await MapToGuideVMAsync(relatedGuide, currentUserId));
        }

        var authorOtherGuideVMs = new List<GuideVM>();
        foreach (var authorGuide in authorOtherGuides.Where(g => g.Id != id))
        {
            authorOtherGuideVMs.Add(await MapToGuideVMAsync(authorGuide, currentUserId));
        }

        return new GuideDetailVM
        {
            Guide = guideVM,
            RelatedGuides = relatedGuideVMs,
            AuthorOtherGuides = authorOtherGuideVMs,
            CanEdit = currentUserId.HasValue && (currentUserId.Value == guide.AuthorId || await IsAdminAsync(currentUserId.Value)),
            CanDelete = currentUserId.HasValue && (currentUserId.Value == guide.AuthorId || await IsAdminAsync(currentUserId.Value)),
            CanVerify = currentUserId.HasValue && await IsAdminAsync(currentUserId.Value),
            CanFeature = currentUserId.HasValue && await IsAdminAsync(currentUserId.Value)
        };
    }

    public async Task<GuideListVM> GetGuidesAsync(GuideFilterDTO filter, Guid? currentUserId = null)
    {
        var guides = await _guidesRepository.GetGuidesAsync(filter);
        var totalCount = await _guidesRepository.GetTotalCountAsync(filter);
        var categories = await _guidesRepository.GetCategoriesAsync();
        var popularTags = await _guidesRepository.GetPopularTagsAsync(20);

        var guideVMs = new List<GuideVM>();
        foreach (var guide in guides)
        {
            guideVMs.Add(await MapToGuideVMAsync(guide, currentUserId));
        }

        var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

        return new GuideListVM
        {
            Guides = guideVMs,
            Pagination = new PaginationInfo
            {
                CurrentPage = filter.Page,
                TotalPages = totalPages,
                PageSize = filter.PageSize,
                TotalItems = totalCount,
                HasPreviousPage = filter.Page > 1,
                HasNextPage = filter.Page < totalPages
            },
            Categories = categories.ToList(),
            PopularTags = popularTags.ToList(),
            TotalCount = totalCount,
            CurrentSearch = filter.Search,
            CurrentCategory = filter.Category,
            CurrentDifficulty = filter.Difficulty?.ToString(),
            CurrentSortBy = filter.SortBy
        };
    }

    public async Task<IEnumerable<GuideVM>> GetFeaturedGuidesAsync(int count = 10, Guid? currentUserId = null)
    {
        var guides = await _guidesRepository.GetFeaturedGuidesAsync(count);
        var guideVMs = new List<GuideVM>();
        
        foreach (var guide in guides)
        {
            guideVMs.Add(await MapToGuideVMAsync(guide, currentUserId));
        }
        
        return guideVMs;
    }

    public async Task<IEnumerable<GuideVM>> GetVerifiedGuidesAsync(int count = 10, Guid? currentUserId = null)
    {
        var guides = await _guidesRepository.GetVerifiedGuidesAsync(count);
        var guideVMs = new List<GuideVM>();
        
        foreach (var guide in guides)
        {
            guideVMs.Add(await MapToGuideVMAsync(guide, currentUserId));
        }
        
        return guideVMs;
    }

    public async Task<IEnumerable<GuideVM>> GetPopularGuidesAsync(int count = 10, Guid? currentUserId = null)
    {
        var guides = await _guidesRepository.GetPopularGuidesAsync(count);
        var guideVMs = new List<GuideVM>();
        
        foreach (var guide in guides)
        {
            guideVMs.Add(await MapToGuideVMAsync(guide, currentUserId));
        }
        
        return guideVMs;
    }

    public async Task<IEnumerable<GuideVM>> GetRecentGuidesAsync(int count = 10, Guid? currentUserId = null)
    {
        var guides = await _guidesRepository.GetRecentGuidesAsync(count);
        var guideVMs = new List<GuideVM>();
        
        foreach (var guide in guides)
        {
            guideVMs.Add(await MapToGuideVMAsync(guide, currentUserId));
        }
        
        return guideVMs;
    }

    public async Task<IEnumerable<GuideVM>> GetGuidesByAuthorAsync(Guid authorId, int count = 10, Guid? currentUserId = null)
    {
        var guides = await _guidesRepository.GetGuidesByAuthorAsync(authorId, count);
        var guideVMs = new List<GuideVM>();
        
        foreach (var guide in guides)
        {
            guideVMs.Add(await MapToGuideVMAsync(guide, currentUserId));
        }
        
        return guideVMs;
    }

    public async Task<IEnumerable<GuideVM>> SearchGuidesAsync(string searchTerm, int count = 20, Guid? currentUserId = null)
    {
        var guides = await _guidesRepository.SearchGuidesAsync(searchTerm, count);
        var guideVMs = new List<GuideVM>();
        
        foreach (var guide in guides)
        {
            guideVMs.Add(await MapToGuideVMAsync(guide, currentUserId));
        }
        
        return guideVMs;
    }

    public async Task<GuideResultDTO> CreateGuideAsync(CreateGuideDTO dto, Guid authorId)
    {
        try
        {
            var guide = new Guide(
                dto.Title,
                dto.Content,
                authorId,
                dto.Summary,
                dto.Category,
                dto.Difficulty,
                dto.EstimatedMinutes);

            if (!string.IsNullOrWhiteSpace(dto.ThumbnailUrl))
                guide.UpdateThumbnail(dto.ThumbnailUrl);

            if (!string.IsNullOrWhiteSpace(dto.CoverImageUrl))
                guide.UpdateCoverImage(dto.CoverImageUrl);

            foreach (var tag in dto.Tags.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                guide.AddTag(tag.Trim());
            }

            foreach (var prerequisite in dto.Prerequisites.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                guide.AddPrerequisite(prerequisite.Trim());
            }

            foreach (var tool in dto.RequiredTools.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                guide.AddRequiredTool(tool.Trim());
            }

            var createdGuide = await _guidesRepository.CreateGuideAsync(guide);

            return new GuideResultDTO
            {
                Success = true,
                Message = "Guide created successfully",
                GuideId = createdGuide.Id
            };
        }
        catch (Exception ex)
        {
            return new GuideResultDTO
            {
                Success = false,
                Message = $"Failed to create guide: {ex.Message}"
            };
        }
    }

    public async Task<GuideResultDTO> UpdateGuideAsync(UpdateGuideDTO dto, Guid currentUserId)
    {
        try
        {
            var guide = await _guidesRepository.GetGuideByIdAsync(dto.Id);
            if (guide == null)
            {
                return new GuideResultDTO
                {
                    Success = false,
                    Message = "Guide not found"
                };
            }

            if (guide.AuthorId != currentUserId && !await IsAdminAsync(currentUserId))
            {
                return new GuideResultDTO
                {
                    Success = false,
                    Message = "You don't have permission to edit this guide"
                };
            }

            guide.UpdateBasicInfo(dto.Title, dto.Content, dto.Summary);
            guide.UpdateCategory(dto.Category);
            guide.UpdateDifficulty(dto.Difficulty);
            guide.UpdateEstimatedTime(dto.EstimatedMinutes);
            guide.UpdateThumbnail(dto.ThumbnailUrl);
            guide.UpdateCoverImage(dto.CoverImageUrl);

            // Update tags, prerequisites, and tools
            // Note: This is a simplified approach. In a real app, you might want more sophisticated collection management
            foreach (var tag in dto.Tags.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                guide.AddTag(tag.Trim());
            }

            foreach (var prerequisite in dto.Prerequisites.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                guide.AddPrerequisite(prerequisite.Trim());
            }

            foreach (var tool in dto.RequiredTools.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                guide.AddRequiredTool(tool.Trim());
            }

            await _guidesRepository.UpdateGuideAsync(guide);

            // Send notification
            var author = await _userManager.FindByIdAsync(guide.AuthorId.ToString());
            if (author != null)
            {
                await _notificationService.NotifyGuideUpdatedAsync(guide, author.FullName ?? author.UserName);
            }

            return new GuideResultDTO
            {
                Success = true,
                Message = "Guide updated successfully",
                GuideId = guide.Id
            };
        }
        catch (Exception ex)
        {
            return new GuideResultDTO
            {
                Success = false,
                Message = $"Failed to update guide: {ex.Message}"
            };
        }
    }

    public async Task<GuideResultDTO> DeleteGuideAsync(Guid id, Guid currentUserId)
    {
        try
        {
            var guide = await _guidesRepository.GetGuideByIdAsync(id);
            if (guide == null)
            {
                return new GuideResultDTO
                {
                    Success = false,
                    Message = "Guide not found"
                };
            }

            if (guide.AuthorId != currentUserId && !await IsAdminAsync(currentUserId))
            {
                return new GuideResultDTO
                {
                    Success = false,
                    Message = "You don't have permission to delete this guide"
                };
            }

            // Store guide info for notification before deletion
            var guideTitle = guide.Title;
            var authorId = guide.AuthorId;
            var author = await _userManager.FindByIdAsync(authorId.ToString());

            await _guidesRepository.DeleteGuideAsync(id);

            // Send notification
            if (author != null)
            {
                await _notificationService.NotifyGuideDeletedAsync(
                    id, 
                    guideTitle, 
                    authorId, 
                    author.FullName ?? author.UserName);
            }

            return new GuideResultDTO
            {
                Success = true,
                Message = "Guide deleted successfully"
            };
        }
        catch (Exception ex)
        {
            return new GuideResultDTO
            {
                Success = false,
                Message = $"Failed to delete guide: {ex.Message}"
            };
        }
    }

    public async Task<GuideResultDTO> PublishGuideAsync(Guid id, Guid currentUserId)
    {
        try
        {
            var guide = await _guidesRepository.GetGuideByIdAsync(id);
            if (guide == null)
            {
                return new GuideResultDTO { Success = false, Message = "Guide not found" };
            }

            if (guide.AuthorId != currentUserId && !await IsAdminAsync(currentUserId))
            {
                return new GuideResultDTO { Success = false, Message = "You don't have permission to publish this guide" };
            }

            guide.Publish();
            await _guidesRepository.UpdateGuideAsync(guide);

            // Send notification
            var author = await _userManager.FindByIdAsync(guide.AuthorId.ToString());
            if (author != null)
            {
                await _notificationService.NotifyGuidePublishedAsync(guide, author.FullName ?? author.UserName);
            }

            return new GuideResultDTO { Success = true, Message = "Guide published successfully", GuideId = guide.Id };
        }
        catch (Exception ex)
        {
            return new GuideResultDTO { Success = false, Message = $"Failed to publish guide: {ex.Message}" };
        }
    }

    public async Task<GuideResultDTO> UnpublishGuideAsync(Guid id, Guid currentUserId)
    {
        try
        {
            var guide = await _guidesRepository.GetGuideByIdAsync(id);
            if (guide == null)
            {
                return new GuideResultDTO { Success = false, Message = "Guide not found" };
            }

            if (guide.AuthorId != currentUserId && !await IsAdminAsync(currentUserId))
            {
                return new GuideResultDTO { Success = false, Message = "You don't have permission to unpublish this guide" };
            }

            guide.Unpublish();
            await _guidesRepository.UpdateGuideAsync(guide);

            return new GuideResultDTO { Success = true, Message = "Guide unpublished successfully", GuideId = guide.Id };
        }
        catch (Exception ex)
        {
            return new GuideResultDTO { Success = false, Message = $"Failed to unpublish guide: {ex.Message}" };
        }
    }

    public async Task<GuideResultDTO> VerifyGuideAsync(Guid id, Guid currentUserId)
    {
        try
        {
            if (!await IsAdminAsync(currentUserId))
            {
                return new GuideResultDTO { Success = false, Message = "You don't have permission to verify guides" };
            }

            var guide = await _guidesRepository.GetGuideByIdAsync(id);
            if (guide == null)
            {
                return new GuideResultDTO { Success = false, Message = "Guide not found" };
            }

            guide.Verify();
            await _guidesRepository.UpdateGuideAsync(guide);

            // Send notification
            var author = await _userManager.FindByIdAsync(guide.AuthorId.ToString());
            if (author != null)
            {
                await _notificationService.NotifyGuideVerifiedAsync(guide, author.FullName ?? author.UserName);
            }

            return new GuideResultDTO { Success = true, Message = "Guide verified successfully", GuideId = guide.Id };
        }
        catch (Exception ex)
        {
            return new GuideResultDTO { Success = false, Message = $"Failed to verify guide: {ex.Message}" };
        }
    }

    public async Task<GuideResultDTO> FeatureGuideAsync(Guid id, Guid currentUserId)
    {
        try
        {
            if (!await IsAdminAsync(currentUserId))
            {
                return new GuideResultDTO { Success = false, Message = "You don't have permission to feature guides" };
            }

            var guide = await _guidesRepository.GetGuideByIdAsync(id);
            if (guide == null)
            {
                return new GuideResultDTO { Success = false, Message = "Guide not found" };
            }

            guide.Feature();
            await _guidesRepository.UpdateGuideAsync(guide);

            // Send notification
            var author = await _userManager.FindByIdAsync(guide.AuthorId.ToString());
            if (author != null)
            {
                await _notificationService.NotifyGuideFeaturedAsync(guide, author.FullName ?? author.UserName);
            }

            return new GuideResultDTO { Success = true, Message = "Guide featured successfully", GuideId = guide.Id };
        }
        catch (Exception ex)
        {
            return new GuideResultDTO { Success = false, Message = $"Failed to feature guide: {ex.Message}" };
        }
    }

    public async Task<GuideResultDTO> UnfeatureGuideAsync(Guid id, Guid currentUserId)
    {
        try
        {
            if (!await IsAdminAsync(currentUserId))
            {
                return new GuideResultDTO { Success = false, Message = "You don't have permission to unfeature guides" };
            }

            var guide = await _guidesRepository.GetGuideByIdAsync(id);
            if (guide == null)
            {
                return new GuideResultDTO { Success = false, Message = "Guide not found" };
            }

            guide.Unfeature();
            await _guidesRepository.UpdateGuideAsync(guide);

            return new GuideResultDTO { Success = true, Message = "Guide unfeatured successfully", GuideId = guide.Id };
        }
        catch (Exception ex)
        {
            return new GuideResultDTO { Success = false, Message = $"Failed to unfeature guide: {ex.Message}" };
        }
    }

    public async Task<GuideResultDTO> BookmarkGuideAsync(Guid guideId, Guid userId)
    {
        try
        {
            var guide = await _guidesRepository.GetGuideByIdAsync(guideId);
            if (guide == null)
            {
                return new GuideResultDTO { Success = false, Message = "Guide not found" };
            }

            var existingBookmark = await _unitOfWork.Bookmarks.GetUserBookmarkAsync(guideId, EntityType.Guide, userId);
            if (existingBookmark != null)
            {
                return new GuideResultDTO { Success = false, Message = "Guide already bookmarked" };
            }

            var bookmark = new Bookmark(guideId, EntityType.Guide, userId);
            await _unitOfWork.Bookmarks.AddAsync(bookmark);
            
            guide.IncrementBookmarkCount();
            await _guidesRepository.UpdateGuideAsync(guide);

            // Send notification to guide author
            var author = await _userManager.FindByIdAsync(guide.AuthorId.ToString());
            var bookmarker = await _userManager.FindByIdAsync(userId.ToString());
            if (author != null && bookmarker != null && guide.AuthorId != userId)
            {
                await _notificationService.NotifyGuideBookmarkedAsync(
                    guide, 
                    author.FullName ?? author.UserName,
                    bookmarker.FullName ?? bookmarker.UserName,
                    userId);
            }

            return new GuideResultDTO { Success = true, Message = "Guide bookmarked successfully" };
        }
        catch (Exception ex)
        {
            return new GuideResultDTO { Success = false, Message = $"Failed to bookmark guide: {ex.Message}" };
        }
    }

    public async Task<GuideResultDTO> UnbookmarkGuideAsync(Guid guideId, Guid userId)
    {
        try
        {
            var guide = await _guidesRepository.GetGuideByIdAsync(guideId);
            if (guide == null)
            {
                return new GuideResultDTO { Success = false, Message = "Guide not found" };
            }

            var bookmark = await _unitOfWork.Bookmarks.GetUserBookmarkAsync(guideId, EntityType.Guide, userId);
            if (bookmark == null)
            {
                return new GuideResultDTO { Success = false, Message = "Guide not bookmarked" };
            }

            await _unitOfWork.Bookmarks.DeleteAsync(bookmark);
            
            guide.DecrementBookmarkCount();
            await _guidesRepository.UpdateGuideAsync(guide);

            return new GuideResultDTO { Success = true, Message = "Guide unbookmarked successfully" };
        }
        catch (Exception ex)
        {
            return new GuideResultDTO { Success = false, Message = $"Failed to unbookmark guide: {ex.Message}" };
        }
    }

    public async Task<GuideResultDTO> RateGuideAsync(Guid guideId, Guid userId, double rating)
    {
        try
        {
            if (rating < 1 || rating > 5)
            {
                return new GuideResultDTO { Success = false, Message = "Rating must be between 1 and 5" };
            }

            var guide = await _guidesRepository.GetGuideByIdAsync(guideId);
            if (guide == null)
            {
                return new GuideResultDTO { Success = false, Message = "Guide not found" };
            }

            var existingRating = await _unitOfWork.Ratings.GetUserRatingAsync(guideId, EntityType.Guide, userId);
            if (existingRating != null)
            {
                // Update existing rating
                existingRating.UpdateValue((int)rating, null);
                await _unitOfWork.Ratings.UpdateAsync(existingRating);
            }
            else
            {
                // Create new rating
                var newRating = new Rating(guideId, EntityType.Guide, userId, (int)rating);
                await _unitOfWork.Ratings.AddAsync(newRating);
                guide.AddRating(rating);
            }

            await _guidesRepository.UpdateGuideAsync(guide);

            // Send notification to guide author
            var author = await _userManager.FindByIdAsync(guide.AuthorId.ToString());
            var rater = await _userManager.FindByIdAsync(userId.ToString());
            if (author != null && rater != null && guide.AuthorId != userId)
            {
                await _notificationService.NotifyGuideRatedAsync(
                    guide,
                    author.FullName ?? author.UserName,
                    rater.FullName ?? rater.UserName,
                    userId,
                    rating);
            }

            return new GuideResultDTO { Success = true, Message = "Guide rated successfully" };
        }
        catch (Exception ex)
        {
            return new GuideResultDTO { Success = false, Message = $"Failed to rate guide: {ex.Message}" };
        }
    }

    public async Task IncrementViewCountAsync(Guid guideId)
    {
        await _guidesRepository.IncrementViewCountAsync(guideId);
    }

    public async Task<GuideCreateVM> GetCreateViewModelAsync()
    {
        return new GuideCreateVM();
    }

    private async Task<GuideVM> MapToGuideVMAsync(Guide guide, Guid? currentUserId = null)
    {
        var author = await _userManager.FindByIdAsync(guide.AuthorId.ToString());
        var isBookmarked = currentUserId.HasValue && await _guidesRepository.IsBookmarkedByUserAsync(guide.Id, currentUserId.Value);
        var userRating = currentUserId.HasValue ? await _guidesRepository.GetUserRatingAsync(guide.Id, currentUserId.Value) : null;

        return new GuideVM
        {
            Id = guide.Id,
            Title = guide.Title,
            Content = guide.Content,
            Summary = guide.Summary,
            AuthorId = guide.AuthorId,
            AuthorName = author?.UserName ?? "Unknown",
            AuthorAvatar = author?.ProfilePictureUrl,
            IsPublished = guide.IsPublished,
            PublishedAt = guide.PublishedAt,
            IsVerified = guide.IsVerified,
            IsFeatured = guide.IsFeatured,
            Category = guide.Category,
            Difficulty = guide.Difficulty,
            EstimatedMinutes = guide.EstimatedMinutes,
            ThumbnailUrl = guide.ThumbnailUrl,
            CoverImageUrl = guide.CoverImageUrl,
            ViewCount = guide.ViewCount,
            BookmarkCount = guide.BookmarkCount,
            AverageRating = guide.AverageRating,
            RatingCount = guide.RatingCount,
            Tags = guide.Tags.ToList(),
            Prerequisites = guide.Prerequisites.ToList(),
            RequiredTools = guide.RequiredTools.ToList(),
            CreatedAt = guide.CreatedAt,
            UpdatedAt = guide.UpdatedAt,
            TimeAgo = GetTimeAgo(guide.PublishedAt ?? guide.CreatedAt),
            IsBookmarked = isBookmarked,
            UserRating = userRating
        };
    }

    private async Task<bool> IsAdminAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        
        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains("Admin") || roles.Contains("Moderator");
    }

    private static string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalDays >= 365)
            return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) == 1 ? "" : "s")} ago";
        if (timeSpan.TotalDays >= 30)
            return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) == 1 ? "" : "s")} ago";
        if (timeSpan.TotalDays >= 1)
            return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays == 1 ? "" : "s")} ago";
        if (timeSpan.TotalHours >= 1)
            return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours == 1 ? "" : "s")} ago";
        if (timeSpan.TotalMinutes >= 1)
            return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes == 1 ? "" : "s")} ago";
        
        return "Just now";
    }
}