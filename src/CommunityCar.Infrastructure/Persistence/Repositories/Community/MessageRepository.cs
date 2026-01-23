using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Domain.Entities.Chats;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class MessageRepository : BaseRepository<Message>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Message>> GetConversationMessagesAsync(Guid conversationId, int page = 1, int pageSize = 50)
    {
        return await Context.Set<Message>()
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadMessageCountAsync(Guid userId)
    {
        var userConversations = await Context.Set<ConversationParticipant>()
            .Where(p => p.UserId == userId)
            .Select(p => p.ConversationId)
            .ToListAsync();

        var unreadCount = 0;
        foreach (var conversationId in userConversations)
        {
            unreadCount += await GetConversationUnreadCountAsync(conversationId, userId);
        }

        return unreadCount;
    }

    public async Task<int> GetConversationUnreadCountAsync(Guid conversationId, Guid userId)
    {
        var participant = await Context.Set<ConversationParticipant>()
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (participant == null) return 0;

        var lastReadAt = participant.LastReadAt ?? participant.JoinedAt;

        return await Context.Set<Message>()
            .CountAsync(m => m.ConversationId == conversationId && 
                           m.CreatedAt > lastReadAt && 
                           m.SenderId != userId);
    }

    public async Task MarkConversationAsReadAsync(Guid conversationId, Guid userId)
    {
        var participant = await Context.Set<ConversationParticipant>()
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (participant != null)
        {
            participant.UpdateLastRead();
        }
    }

    public async Task<Message?> GetLastMessageAsync(Guid conversationId)
    {
        return await Context.Set<Message>()
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync();
    }
}