using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Domain.Entities.Community.Posts;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class PostsRepository : BaseRepository<Post>, IPostsRepository
{
    public PostsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Post?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await Context.Posts
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
    }

    public async Task<Post?> GetBySlugIncludeDeletedAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await Context.Posts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
    }

    public async Task<(IEnumerable<Post> Items, int TotalCount)> SearchAsync(
        string? searchTerm, 
        PostType? type, 
        Guid? authorId, 
        Guid? groupId, 
        string? sortBy, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var query = Context.Posts.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
            query = query.Where(p => p.Title.Contains(searchTerm) || p.Content.Contains(searchTerm));

        if (type.HasValue)
            query = query.Where(p => p.Type == type.Value);

        if (authorId.HasValue)
            query = query.Where(p => p.AuthorId == authorId.Value);

        if (groupId.HasValue)
            query = query.Where(p => p.GroupId == groupId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IEnumerable<Post> Items, int TotalCount)> SearchIncludeDeletedAsync(
        string? searchTerm, 
        PostType? type, 
        Guid? authorId, 
        Guid? groupId, 
        string? sortBy, 
        int page, 
        int pageSize, 
        bool includeDeleted = false,
        bool deletedOnly = false,
        CancellationToken cancellationToken = default)
    {
        var query = includeDeleted || deletedOnly ? Context.Posts.IgnoreQueryFilters() : Context.Posts.AsQueryable();

        if (deletedOnly)
            query = query.Where(p => p.IsDeleted);
        else if (!includeDeleted)
            query = query.Where(p => !p.IsDeleted);

        if (!string.IsNullOrEmpty(searchTerm))
            query = query.Where(p => p.Title.Contains(searchTerm) || p.Content.Contains(searchTerm));

        if (type.HasValue)
            query = query.Where(p => p.Type == type.Value);

        if (authorId.HasValue)
            query = query.Where(p => p.AuthorId == authorId.Value);

        if (groupId.HasValue)
            query = query.Where(p => p.GroupId == groupId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<Post>> GetByAuthorAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Posts
            .Where(p => p.AuthorId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetByAuthorIncludeDeletedAsync(Guid userId, bool includeDeleted = false, bool deletedOnly = false, CancellationToken cancellationToken = default)
    {
        var query = includeDeleted || deletedOnly ? Context.Posts.IgnoreQueryFilters() : Context.Posts.AsQueryable();

        if (deletedOnly)
            query = query.Where(p => p.IsDeleted && p.AuthorId == userId);
        else if (!includeDeleted)
            query = query.Where(p => !p.IsDeleted && p.AuthorId == userId);
        else
            query = query.Where(p => p.AuthorId == userId);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetByGroupAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await Context.Posts
            .Where(p => p.GroupId == groupId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetByGroupIncludeDeletedAsync(Guid groupId, bool includeDeleted = false, bool deletedOnly = false, CancellationToken cancellationToken = default)
    {
        var query = includeDeleted || deletedOnly ? Context.Posts.IgnoreQueryFilters() : Context.Posts.AsQueryable();

        if (deletedOnly)
            query = query.Where(p => p.IsDeleted && p.GroupId == groupId);
        else if (!includeDeleted)
            query = query.Where(p => !p.IsDeleted && p.GroupId == groupId);
        else
            query = query.Where(p => p.GroupId == groupId);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetRecentPostsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await Context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetRecentPostsIncludeDeletedAsync(int count, bool includeDeleted = false, bool deletedOnly = false, CancellationToken cancellationToken = default)
    {
        var query = includeDeleted || deletedOnly ? Context.Posts.IgnoreQueryFilters() : Context.Posts.AsQueryable();

        if (deletedOnly)
            query = query.Where(p => p.IsDeleted);
        else if (!includeDeleted)
            query = query.Where(p => !p.IsDeleted);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUserPostsCountAsync(Guid userId)
    {
        return await Context.Posts.CountAsync(p => p.AuthorId == userId);
    }

    public async Task<int> GetUserPostsCountIncludeDeletedAsync(Guid userId, bool includeDeleted = false, bool deletedOnly = false)
    {
        var query = includeDeleted || deletedOnly ? Context.Posts.IgnoreQueryFilters() : Context.Posts.AsQueryable();

        if (deletedOnly)
            query = query.Where(p => p.IsDeleted && p.AuthorId == userId);
        else if (!includeDeleted)
            query = query.Where(p => !p.IsDeleted && p.AuthorId == userId);
        else
            query = query.Where(p => p.AuthorId == userId);

        return await query.CountAsync();
    }

    public async Task<int> SoftDeletePostsByAuthorAsync(Guid authorId, string? deletedBy = null)
    {
        var posts = await Context.Posts
            .Where(p => p.AuthorId == authorId && !p.IsDeleted)
            .ToListAsync();

        foreach (var post in posts)
        {
            post.SoftDelete(deletedBy);
        }

        return await Context.SaveChangesAsync();
    }

    public async Task<int> SoftDeletePostsByGroupAsync(Guid groupId, string? deletedBy = null)
    {
        var posts = await Context.Posts
            .Where(p => p.GroupId == groupId && !p.IsDeleted)
            .ToListAsync();

        foreach (var post in posts)
        {
            post.SoftDelete(deletedBy);
        }

        return await Context.SaveChangesAsync();
    }

    public async Task<int> RestorePostsByAuthorAsync(Guid authorId, string? restoredBy = null)
    {
        var posts = await Context.Posts
            .IgnoreQueryFilters()
            .Where(p => p.AuthorId == authorId && p.IsDeleted)
            .ToListAsync();

        foreach (var post in posts)
        {
            post.Restore(restoredBy);
        }

        return await Context.SaveChangesAsync();
    }

    public async Task<int> RestorePostsByGroupAsync(Guid groupId, string? restoredBy = null)
    {
        var posts = await Context.Posts
            .IgnoreQueryFilters()
            .Where(p => p.GroupId == groupId && p.IsDeleted)
            .ToListAsync();

        foreach (var post in posts)
        {
            post.Restore(restoredBy);
        }

        return await Context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Post>> GetDeletedPostsForModerationAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return await Context.Posts
            .IgnoreQueryFilters()
            .Where(p => p.IsDeleted)
            .OrderByDescending(p => p.DeletedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetDeletedPostsCountAsync()
    {
        return await Context.Posts
            .IgnoreQueryFilters()
            .CountAsync(p => p.IsDeleted);
    }
}
