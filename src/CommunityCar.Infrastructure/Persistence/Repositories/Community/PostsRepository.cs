using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Domain.Entities.Community.Posts;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class PostsRepository : IPostsRepository
{
    private readonly ApplicationDbContext _context;

    public PostsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Post?> GetByIdAsync(Guid id)
    {
        return await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _context.Posts.ToListAsync();
    }

    public async Task<(IEnumerable<Post> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        PostType? type = null,
        Guid? authorId = null,
        Guid? groupId = null,
        string? sortBy = "newest",
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Posts.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.ToLower();
            query = query.Where(p => 
                p.Title.ToLower().Contains(search) ||
                p.Content.ToLower().Contains(search));
        }

        if (type.HasValue)
        {
            query = query.Where(p => p.Type == type.Value);
        }

        if (authorId.HasValue)
        {
            query = query.Where(p => p.AuthorId == authorId.Value);
        }

        // Apply sorting
        query = sortBy?.ToLower() switch
        {
            "title" => query.OrderBy(p => p.Title),
            "oldest" => query.OrderBy(p => p.CreatedAt),
            "newest" or _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<Post>> GetByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        return await _context.Posts
            .Where(p => p.AuthorId == authorId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetByGroupAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        // Note: This will need to be updated when Group-Post relationship is implemented
        return await _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetRecentPostsAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUserPostsCountAsync(Guid userId)
    {
        return await _context.Posts
            .CountAsync(p => p.AuthorId == userId);
    }

    public async Task AddAsync(Post post)
    {
        await _context.Posts.AddAsync(post);
    }

    public Task UpdateAsync(Post post)
    {
        _context.Posts.Update(post);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Post post)
    {
        _context.Posts.Remove(post);
        return Task.CompletedTask;
    }
}
