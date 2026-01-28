using System;
using System.Collections.Generic;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Community.Reviews;

public class Review : AggregateRoot
{
    public Guid TargetId { get; private set; } // Could be a car, a shop, etc.
    public string TargetType { get; private set; } // discriminator
    public int Rating { get; private set; } // 1-5
    public string Title { get; private set; }
    public string Comment { get; private set; }
    
    // Arabic Localization
    public string? TitleAr { get; private set; }
    public string? CommentAr { get; private set; }
    
    public Guid ReviewerId { get; private set; }
    
    // Enhanced properties
    public bool IsVerifiedPurchase { get; private set; }
    public bool IsRecommended { get; private set; }
    public DateTime? PurchaseDate { get; private set; }
    public decimal? PurchasePrice { get; private set; }
    
    // Engagement metrics
    public int HelpfulCount { get; private set; }
    public int NotHelpfulCount { get; private set; }
    public int ReplyCount { get; private set; }
    public int ViewCount { get; private set; }
    
    // Review status
    public bool IsApproved { get; private set; }
    public bool IsFlagged { get; private set; }
    public bool IsEdited { get; private set; }
    public DateTime? EditedAt { get; private set; }
    
    // Automotive specific
    public string? CarMake { get; private set; }
    public string? CarModel { get; private set; }
    public int? CarYear { get; private set; }
    public int? Mileage { get; private set; }
    public string? OwnershipDuration { get; private set; }
    
    // Detailed ratings (1-5 each)
    public int? QualityRating { get; private set; }
    public int? ValueRating { get; private set; }
    public int? ReliabilityRating { get; private set; }
    public int? PerformanceRating { get; private set; }
    public int? ComfortRating { get; private set; }
    
    // Media
    private readonly List<string> _imageUrls = new();
    public IReadOnlyCollection<string> ImageUrls => _imageUrls.AsReadOnly();
    
    // Pros and cons
    private readonly List<string> _pros = new();
    private readonly List<string> _cons = new();
    public IReadOnlyCollection<string> Pros => _pros.AsReadOnly();
    public IReadOnlyCollection<string> Cons => _cons.AsReadOnly();

    public Review(Guid targetId, string targetType, int rating, string title, string comment, Guid reviewerId)
    {
        TargetId = targetId;
        TargetType = targetType;
        Rating = rating;
        Title = title;
        Comment = comment;
        ReviewerId = reviewerId;
        IsApproved = false;
        IsFlagged = false;
        IsEdited = false;
        IsVerifiedPurchase = false;
        IsRecommended = false;
        HelpfulCount = 0;
        NotHelpfulCount = 0;
        ReplyCount = 0;
        ViewCount = 0;
    }

    private Review() { }

    public void UpdateContent(string title, string comment)
    {
        Title = title;
        Comment = comment;
        IsEdited = true;
        EditedAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void UpdateArabicContent(string? titleAr, string? commentAr)
    {
        TitleAr = titleAr;
        CommentAr = commentAr;
        IsEdited = true;
        EditedAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void UpdateRating(int rating)
    {
        Rating = rating;
        IsEdited = true;
        EditedAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void SetDetailedRatings(int? quality, int? value, int? reliability, int? performance, int? comfort)
    {
        QualityRating = quality;
        ValueRating = value;
        ReliabilityRating = reliability;
        PerformanceRating = performance;
        ComfortRating = comfort;
        Audit(UpdatedBy);
    }

    public void SetPurchaseInfo(bool isVerifiedPurchase, DateTime? purchaseDate = null, decimal? purchasePrice = null)
    {
        IsVerifiedPurchase = isVerifiedPurchase;
        PurchaseDate = purchaseDate;
        PurchasePrice = purchasePrice;
        Audit(UpdatedBy);
    }

    public void SetCarInfo(string? carMake, string? carModel, int? carYear, int? mileage, string? ownershipDuration)
    {
        CarMake = carMake;
        CarModel = carModel;
        CarYear = carYear;
        Mileage = mileage;
        OwnershipDuration = ownershipDuration;
        Audit(UpdatedBy);
    }

    public void SetRecommendation(bool isRecommended)
    {
        IsRecommended = isRecommended;
        Audit(UpdatedBy);
    }

    public void AddImage(string imageUrl)
    {
        if (!_imageUrls.Contains(imageUrl))
        {
            _imageUrls.Add(imageUrl);
            Audit(UpdatedBy);
        }
    }

    public void RemoveImage(string imageUrl)
    {
        if (_imageUrls.Remove(imageUrl))
        {
            Audit(UpdatedBy);
        }
    }

    public void AddPro(string pro)
    {
        if (!_pros.Contains(pro))
        {
            _pros.Add(pro);
            Audit(UpdatedBy);
        }
    }

    public void RemovePro(string pro)
    {
        if (_pros.Remove(pro))
        {
            Audit(UpdatedBy);
        }
    }

    public void AddCon(string con)
    {
        if (!_cons.Contains(con))
        {
            _cons.Add(con);
            Audit(UpdatedBy);
        }
    }

    public void RemoveCon(string con)
    {
        if (_cons.Remove(con))
        {
            Audit(UpdatedBy);
        }
    }

    public void Approve()
    {
        IsApproved = true;
        IsFlagged = false;
        Audit(UpdatedBy);
    }

    public void Flag()
    {
        IsFlagged = true;
        Audit(UpdatedBy);
    }

    public void Unflag()
    {
        IsFlagged = false;
        Audit(UpdatedBy);
    }

    public void IncrementHelpfulCount()
    {
        HelpfulCount++;
    }

    public void DecrementHelpfulCount()
    {
        if (HelpfulCount > 0)
            HelpfulCount--;
    }

    public void IncrementNotHelpfulCount()
    {
        NotHelpfulCount++;
    }

    public void DecrementNotHelpfulCount()
    {
        if (NotHelpfulCount > 0)
            NotHelpfulCount--;
    }

    public void IncrementReplyCount()
    {
        ReplyCount++;
    }

    public void DecrementReplyCount()
    {
        if (ReplyCount > 0)
            ReplyCount--;
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }

    public double AverageDetailedRating
    {
        get
        {
            var ratings = new[] { QualityRating, ValueRating, ReliabilityRating, PerformanceRating, ComfortRating }
                .Where(r => r.HasValue)
                .Select(r => r.Value)
                .ToArray();

            return ratings.Any() ? ratings.Average() : Rating;
        }
    }

    public int HelpfulnessScore => HelpfulCount - NotHelpfulCount;

    public string CarDisplayName => 
        !string.IsNullOrEmpty(CarMake) && !string.IsNullOrEmpty(CarModel) 
            ? $"{CarYear} {CarMake} {CarModel}".Trim()
            : !string.IsNullOrEmpty(CarMake) 
                ? CarMake 
                : string.Empty;
}
