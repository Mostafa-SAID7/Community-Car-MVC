using CommunityCar.Web.Areas.Communication.Interfaces.Repositories.Chat;

namespace CommunityCar.Web.Areas.Communication.Interfaces.Repositories;

public interface ICommunicationUnitOfWork : IDisposable
{
    IConversationParticipantRepository ConversationParticipants { get; }
    IConversationRepository Conversations { get; }
    IMessageRepository Messages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
