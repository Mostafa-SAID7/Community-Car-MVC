using CommunityCar.Web.Areas.Communication.Interfaces.Repositories;
using CommunityCar.Web.Areas.Communication.Interfaces.Repositories.Chat;
using CommunityCar.Infrastructure.Persistence.Data;

namespace CommunityCar.Web.Areas.Communication.Repositories;

public class CommunicationUnitOfWork : ICommunicationUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public CommunicationUnitOfWork(
        ApplicationDbContext context,
        IConversationParticipantRepository conversationParticipantRepository,
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository)
    {
        _context = context;
        ConversationParticipants = conversationParticipantRepository;
        Conversations = conversationRepository;
        Messages = messageRepository;
    }

    public IConversationParticipantRepository ConversationParticipants { get; }
    public IConversationRepository Conversations { get; }
    public IMessageRepository Messages { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
