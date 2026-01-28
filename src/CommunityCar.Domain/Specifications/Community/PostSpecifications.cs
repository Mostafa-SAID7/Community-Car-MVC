using CommunityCar.Domain.Entities.Community.Posts;

namespace CommunityCar.Domain.Specifications.Community;

/// <summary>
/// Specifications for Post entity queries
/// </summary>
public static class PostSpecifications
{
    /// <summary>
    /// Specification for published posts
    /// </summary>
    public class PublishedPostsSpec : BaseSpecification<Post>
    {
        public PublishedPostsSpec() : base(p => p.IsPublished && !p.IsDeleted)
        {
            AddOrderByDescending(p => p.CreatedAt);
        }
    }

    /// <summary>
    /// Specification for posts by author
    /// </summary>
    public class PostsByAuthorSpec : BaseSpecification<Post>
    {
        public PostsByAuthorSpec(Guid authorId) : base(p => p.AuthorId == authorId && !p.IsDeleted)
        {
            AddOrderByDescending(p => p.CreatedAt);
        }
    }

    /// <summary>
    /// Specification for posts by category
    /// </summary>
    public class PostsByCategorySpec : BaseSpecification<Post>
    {
        public PostsByCategorySpec(Guid categoryId) 
            : base(p => p.CategoryId == categoryId && p.IsPublished && !p.IsDeleted)
        {
            AddOrderByDescending(p => p.CreatedAt);
        }
    }

    /// <summary>
    /// Specification for featured posts
    /// </summary>
    public class FeaturedPostsSpec : BaseSpecification<Post>
    {
        public FeaturedPostsSpec() : base(p => p.IsFeatured && p.IsPublished && !p.IsDeleted)
        {
            AddOrderByDescending(p => p.CreatedAt);
        }
    }

    /// <summary>
    /// Specification for popular posts (by view count)
    /// </summary>
    public class PopularPostsSpec : BaseSpecification<Post>
    {
        public PopularPostsSpec(int minViews = 100) 
            : base(p => p.ViewCount >= minViews && p.IsPublished && !p.IsDeleted)
        {
            AddOrderByDescending(p => p.ViewCount);
        }
    }

    /// <summary>
    /// Specification for recent posts
    /// </summary>
    public class RecentPostsSpec : BaseSpecification<Post>
    {
        public RecentPostsSpec(int daysBack = 7) 
            : base(p => p.CreatedAt >= DateTime.UtcNow.AddDays(-daysBack) && 
                       p.IsPublished && !p.IsDeleted)
        {
            AddOrderByDescending(p => p.CreatedAt);
        }
    }

    /// <summary>
    /// Specification for posts with high engagement
    /// </summary>
    public class HighEngagementPostsSpec : BaseSpecification<Post>
    {
        public HighEngagementPostsSpec(int minLikes = 10, int minComments = 5) 
            : base(p => p.LikeCount >= minLikes && 
                       p.CommentCount >= minComments && 
                       p.IsPublished && !p.IsDeleted)
        {
            AddOrderByDescending(p => p.LikeCount + p.CommentCount);
        }
    }

    /// <summary>
    /// Specification for searching posts by content
    /// </summary>
    public class PostSearchSpec : BaseSpecification<Post>
    {
        public PostSearchSpec(string searchTerm) 
            : base(p => (p.Title.Contains(searchTerm) || p.Content.Contains(searchTerm)) && 
                       p.IsPublished && !p.IsDeleted)
        {
            AddOrderByDescending(p => p.CreatedAt);
        }
    }

    /// <summary>
    /// Specification for posts with tags
    /// </summary>
    public class PostsWithTagsSpec : BaseSpecification<Post>
    {
        public PostsWithTagsSpec(List<string> tags) 
            : base(p => p.Tags.Any(t => tags.Contains(t.Name)) && 
                       p.IsPublished && !p.IsDeleted)
        {
            AddOrderByDescending(p => p.CreatedAt);
        }
    }
}