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

    public async Task<IEnumerable<Post>> GetByAuthorAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Posts
            .Where(p => p.AuthorId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetByGroupAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await Context.Posts
            .Where(p => p.GroupId == groupId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetRecentPostsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await Context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUserPostsCountAsync(Guid userId)
    {
        return await Context.Posts.CountAsync(p => p.AuthorId == userId);
    }
}
