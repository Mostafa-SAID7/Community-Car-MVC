using Microsoft.EntityFrameworkCore;
using CommunityCar.Application.Common.Interfaces.Repositories.Chat;
using CommunityCar.Domain.Entities.Chats;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;

namespace CommunityCar.Web.Areas.Communication.Repositories.Chat;

public class ConversationParticipantRepository : BaseRepository<ConversationParticipant>, IConversationParticipantRepository
{
    public ConversationParticipantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ConversationParticipant>> GetConversationParticipantsAsync(Guid conversationId)
    {
        return await Context.ConversationParticipants
            .Where(p => p.ConversationId == conversationId && !p.IsDeleted)
            .OrderBy(p => p.JoinedAt)
            .ToListAsync();
    }

    public async Task<ConversationParticipant?> GetParticipantAsync(Guid conversationId, Guid userId)
    {
        return await Context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);
    }

    public async Task<bool> IsParticipantAsync(Guid conversationId, Guid userId)
    {
        return await Context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId && !p.IsDeleted);
    }

    public async Task<bool> UpdateLastReadAsync(Guid conversationId, Guid userId, DateTime lastReadAt)
    {
        var participant = await GetParticipantAsync(conversationId, userId);
        if (participant == null) return false;

        participant.UpdateLastRead(lastReadAt);
        await UpdateAsync(participant);
        return true;
    }

    public async Task<bool> MuteConversationAsync(Guid conversationId, Guid userId, bool isMuted)
    {
        var participant = await GetParticipantAsync(conversationId, userId);
        if (participant == null) return false;

        if (isMuted)
        {
            participant.Mute();
        }
        else
        {
            participant.Unmute();
        }

        await UpdateAsync(participant);
        return true;
    }

    public async Task<bool> SetParticipantRoleAsync(Guid conversationId, Guid userId, string role)
    {
        var participant = await GetParticipantAsync(conversationId, userId);
        if (participant == null) return false;

        participant.SetRole(role);
        await UpdateAsync(participant);
        return true;
    }

    public async Task<IEnumerable<ConversationParticipant>> GetActiveParticipantsAsync(Guid conversationId)
    {
        return await Context.ConversationParticipants
            .Where(p => p.ConversationId == conversationId && !p.IsDeleted && !p.IsArchived)
            .OrderBy(p => p.JoinedAt)
            .ToListAsync();
    }

    public async Task<int> GetActiveParticipantCountAsync(Guid conversationId)
    {
        return await Context.ConversationParticipants
            .Where(p => p.ConversationId == conversationId && !p.IsDeleted && !p.IsArchived)
            .CountAsync();
    }
}
