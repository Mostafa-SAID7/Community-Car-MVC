using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Reviews.DTOs;
using CommunityCar.Application.Features.Reviews.ViewModels;
using CommunityCar.Application.Common.Models;
using CommunityCar.Domain.Entities.Community.Reviews;

namespace CommunityCar.Application.Services.Community;

public class ReviewsService : IReviewsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReviewsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ReviewsSearchResponse> SearchReviewsAsync(ReviewsSearchRequest request)
    {
        var reviews = await _unitOfWork.Reviews.GetAllAsync();
        var queryable = reviews.AsQueryable();

        // Apply search term filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLowerInvariant();
            queryable = queryable.Where(r => 
                r.Title.ToLowerInvariant().Contains(searchTerm) ||
                r.Comment.ToLowerInvariant().Contains(searchTerm) ||
                (r.CarMake != null && r.CarMake.ToLowerInvariant().Contains(searchTerm)) ||
                (r.CarModel != null && r.CarModel.ToLowerInvariant().Contains(searchTerm))
            );
        }

        // Apply filters
        if (request.TargetId.HasValue)
            queryable = queryable.Where(r => r.TargetId == request.TargetId.Value);

        if (!string.IsNullOrWhiteSpace(request.TargetType))
            queryable = queryable.Where(r => r.TargetType == request.TargetType);

        if (request.ReviewerId.HasValue)
            queryable = queryable.Where(r => r.ReviewerId == request.ReviewerId.Value);

        if (request.Rating.HasValue)
            queryable = queryable.Where(r => r.Rating == request.Rating.Value);

        if (request.MinRating.HasValue)
            queryable = queryable.Where(r => r.Rating >= request.MinRating.Value);

        if (request.MaxRating.HasValue)
            queryable = queryable.Where(r => r.Rating <= request.MaxRating.Value);

        if (!string.IsNullOrWhiteSpace(request.CarMake))
            queryable = queryable.Where(r => r.CarMake != null && r.CarMake.ToLowerInvariant() == request.CarMake.ToLowerInvariant());

        if (!string.IsNullOrWhiteSpace(request.CarModel))
            queryable = queryable.Where(r => r.CarModel != null && r.CarModel.ToLowerInvariant() == request.CarModel.ToLowerInvariant());

        if (request.CarYear.HasValue)
            queryable = queryable.Where(r => r.CarYear == request.CarYear.Value);

        if (request.IsVerifiedPurchase.HasValue)
            queryable = queryable.Where(r => r.IsVerifiedPurchase == request.IsVerifiedPurchase.Value);

        if (request.IsRecommended.HasValue)
            queryable = queryable.Where(r => r.IsRecommended == request.IsRecommended.Value);

        if (request.IsApproved.HasValue)
            queryable = queryable.Where(r => r.IsApproved == request.IsApproved.Value);

        if (request.IsFlagged.HasValue)
            queryable = queryable.Where(r => r.IsFlagged == request.IsFlagged.Value);

        // Apply date filters
        if (request.CreatedAfter.HasValue)
            queryable = queryable.Where(r => r.CreatedAt >= request.CreatedAfter.Value);

        if (request.CreatedBefore.HasValue)
            queryable = queryable.Where(r => r.CreatedAt <= request.CreatedBefore.Value);

        // Apply engagement filters
        if (request.MinHelpfulCount.HasValue)
            queryable = queryable.Where(r => r.HelpfulCount >= request.MinHelpfulCount.Value);

        if (request.MaxHelpfulCount.HasValue)
            queryable = queryable.Where(r => r.HelpfulCount <= request.MaxHelpfulCount.Value);

        // Get total count before pagination
        var totalCount = queryable.Count();

        // Apply sorting
        queryable = request.SortBy switch
        {
            ReviewsSortBy.Newest => queryable.OrderByDescending(r => r.CreatedAt),
            ReviewsSortBy.Oldest => queryable.OrderBy(r => r.CreatedAt),
            ReviewsSortBy.HighestRating => queryable.OrderByDescending(r => r.Rating),
            ReviewsSortBy.LowestRating => queryable.OrderBy(r => r.Rating),
            ReviewsSortBy.MostHelpful => queryable.OrderByDescending(r => r.HelpfulCount),
            ReviewsSortBy.LeastHelpful => queryable.OrderBy(r => r.HelpfulCount),
            ReviewsSortBy.MostViews => queryable.OrderByDescending(r => r.ViewCount),
            ReviewsSortBy.Relevance => !string.IsNullOrWhiteSpace(request.SearchTerm) 
                ? queryable.OrderByDescending(r => CalculateRelevanceScore(r, request.SearchTerm))
                : queryable.OrderByDescending(r => r.CreatedAt),
            _ => queryable.OrderByDescending(r => r.CreatedAt)
        };

        // Apply pagination
        var skip = (request.Page - 1) * request.PageSize;
        var pagedReviews = queryable.Skip(skip).Take(request.PageSize).ToList();

        // Map to ViewModels
        var reviewVMs = _mapper.Map<List<ReviewVM>>(pagedReviews);

        // Set computed properties
        foreach (var vm in reviewVMs)
        {
            SetComputedProperties(vm);
        }

        // Calculate pagination info
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
        var pagination = new PaginationInfo
        {
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount,
            TotalPages = totalPages,
            HasPreviousPage = request.Page > 1,
            HasNextPage = request.Page < totalPages,
            StartItem = skip + 1,
            EndItem = Math.Min(skip + request.PageSize, totalCount)
        };

        // Get stats and available filters
        var stats = await GetReviewsStatsAsync();
        var availableCarMakes = await GetAvailableCarMakesAsync();

        return new ReviewsSearchResponse
        {
            Reviews = reviewVMs,
            Pagination = pagination,
            Stats = stats,
            AvailableCarMakes = availableCarMakes
        };
    }

    public async Task<ReviewVM?> GetByIdAsync(Guid id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null) return null;
        
        var vm = _mapper.Map<ReviewVM>(review);
        SetComputedProperties(vm);
        return vm;
    }

    public async Task<ReviewVM> CreateAsync(CreateReviewRequest request)
    {
        var review = new Review(request.TargetId, request.TargetType, request.Rating, request.Title, request.Comment, request.ReviewerId);
        
        if (!string.IsNullOrEmpty(request.TitleAr) || !string.IsNullOrEmpty(request.CommentAr))
        {
            review.UpdateArabicContent(request.TitleAr, request.CommentAr);
        }
        
        review.SetPurchaseInfo(request.IsVerifiedPurchase, request.PurchaseDate, request.PurchasePrice);
        review.SetRecommendation(request.IsRecommended);
        review.SetCarInfo(request.CarMake, request.CarModel, request.CarYear, request.Mileage, request.OwnershipDuration);
        review.SetDetailedRatings(request.QualityRating, request.ValueRating, request.ReliabilityRating, request.PerformanceRating, request.ComfortRating);

        foreach (var imageUrl in request.ImageUrls)
            review.AddImage(imageUrl);

        foreach (var pro in request.Pros)
            review.AddPro(pro);

        foreach (var con in request.Cons)
            review.AddCon(con);

        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ReviewVM>(review);
    }

    public async Task<ReviewVM> UpdateAsync(Guid id, UpdateReviewRequest request)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
            throw new ArgumentException("Review not found");

        review.UpdateContent(request.Title, request.Comment);
        
        if (!string.IsNullOrEmpty(request.TitleAr) || !string.IsNullOrEmpty(request.CommentAr))
        {
            review.UpdateArabicContent(request.TitleAr, request.CommentAr);
        }
        review.UpdateRating(request.Rating);
        review.SetRecommendation(request.IsRecommended);
        review.SetCarInfo(request.CarMake, request.CarModel, request.CarYear, request.Mileage, request.OwnershipDuration);
        review.SetDetailedRatings(request.QualityRating, request.ValueRating, request.ReliabilityRating, request.PerformanceRating, request.ComfortRating);

        // Update images, pros, and cons
        foreach (var imageUrl in request.ImageUrls)
            review.AddImage(imageUrl);

        foreach (var pro in request.Pros)
            review.AddPro(pro);

        foreach (var con in request.Cons)
            review.AddCon(con);

        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ReviewVM>(review);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
            return false;

        await _unitOfWork.Reviews.DeleteAsync(review);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ApproveAsync(Guid id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
            return false;

        review.Approve();
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> FlagAsync(Guid id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
            return false;

        review.Flag();
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnflagAsync(Guid id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
            return false;

        review.Unflag();
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkHelpfulAsync(Guid id, Guid userId, bool isHelpful)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
            return false;

        if (isHelpful)
            review.IncrementHelpfulCount();
        else
            review.IncrementNotHelpfulCount();

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IncrementViewCountAsync(Guid id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
            return false;

        review.IncrementViewCount();
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<string>> GetAvailableCarMakesAsync()
    {
        return await _unitOfWork.Reviews.GetAvailableCarMakesAsync();
    }

    public async Task<ReviewsStatsVM> GetReviewsStatsAsync()
    {
        var allReviews = await _unitOfWork.Reviews.GetAllAsync();
        var thisMonth = allReviews.Where(r => r.CreatedAt >= DateTime.UtcNow.AddMonths(-1));
        var thisWeek = allReviews.Where(r => r.CreatedAt >= DateTime.UtcNow.AddDays(-7));
        var today = allReviews.Where(r => r.CreatedAt.Date == DateTime.UtcNow.Date);

        return new ReviewsStatsVM
        {
            TotalReviews = allReviews.Count(),
            ApprovedReviews = allReviews.Count(r => r.IsApproved),
            PendingReviews = allReviews.Count(r => !r.IsApproved && !r.IsFlagged),
            FlaggedReviews = allReviews.Count(r => r.IsFlagged),
            VerifiedPurchaseReviews = allReviews.Count(r => r.IsVerifiedPurchase),
            TotalViews = allReviews.Sum(r => r.ViewCount),
            TotalHelpfulVotes = allReviews.Sum(r => r.HelpfulCount),
            ReviewsThisMonth = thisMonth.Count(),
            ReviewsThisWeek = thisWeek.Count(),
            ReviewsToday = today.Count(),
            AverageRating = allReviews.Any() ? allReviews.Average(r => r.Rating) : 0,
            AverageHelpfulnessScore = allReviews.Any() ? allReviews.Average(r => r.HelpfulnessScore) : 0,
            AverageViewsPerReview = allReviews.Any() ? allReviews.Average(r => r.ViewCount) : 0
        };
    }

    public async Task<IEnumerable<ReviewVM>> GetReviewsByTargetAsync(Guid targetId, string targetType)
    {
        var reviews = await _unitOfWork.Reviews.GetByTargetAsync(targetId, targetType);
        var vms = _mapper.Map<List<ReviewVM>>(reviews);
        
        foreach (var vm in vms)
        {
            SetComputedProperties(vm);
        }
        
        return vms;
    }

    public async Task<double> GetAverageRatingByTargetAsync(Guid targetId, string targetType)
    {
        return await _unitOfWork.Reviews.GetAverageRatingByTargetAsync(targetId, targetType);
    }

    private static double CalculateRelevanceScore(Review review, string searchTerm)
    {
        var score = 0.0;
        var lowerSearchTerm = searchTerm.ToLowerInvariant();

        if (review.Title.ToLowerInvariant().Contains(lowerSearchTerm))
            score += 10;

        if (review.Comment.ToLowerInvariant().Contains(lowerSearchTerm))
            score += 5;

        if (review.CarMake?.ToLowerInvariant().Contains(lowerSearchTerm) == true ||
            review.CarModel?.ToLowerInvariant().Contains(lowerSearchTerm) == true)
            score += 6;

        score += review.HelpfulCount * 0.5;
        score += review.ViewCount * 0.01;
        score += review.Rating * 0.2;

        return score;
    }

    private static void SetComputedProperties(ReviewVM vm)
    {
        // Set TimeAgo
        vm.TimeAgo = GetTimeAgo(vm.CreatedAt);

        // Set HelpfulnessScore
        vm.HelpfulnessScore = vm.HelpfulCount - vm.NotHelpfulCount;

        // Set HasImages
        vm.HasImages = vm.ImageUrls.Any();

        // Set HasDetailedRatings
        vm.HasDetailedRatings = vm.QualityRating.HasValue || vm.ValueRating.HasValue || 
                               vm.ReliabilityRating.HasValue || vm.PerformanceRating.HasValue || 
                               vm.ComfortRating.HasValue;

        // Set AverageDetailedRating
        if (vm.HasDetailedRatings)
        {
            var ratings = new[] { vm.QualityRating, vm.ValueRating, vm.ReliabilityRating, vm.PerformanceRating, vm.ComfortRating }
                .Where(r => r.HasValue)
                .Select(r => r.Value)
                .ToArray();

            vm.AverageDetailedRating = ratings.Any() ? ratings.Average() : vm.Rating;
        }
        else
        {
            vm.AverageDetailedRating = vm.Rating;
        }

        // Set CarDisplayName
        vm.CarDisplayName = !string.IsNullOrEmpty(vm.CarMake) && !string.IsNullOrEmpty(vm.CarModel) 
            ? $"{vm.CarYear} {vm.CarMake} {vm.CarModel}".Trim()
            : !string.IsNullOrEmpty(vm.CarMake) 
                ? vm.CarMake 
                : string.Empty;
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


