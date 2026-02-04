using CommunityCar.Application.Features.Shared.Interactions.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community.Interactions;

public interface IInteractionService
{
    Task<List<InteractionVM>> GetInteractionsAsync(Guid entityId, string entityType, CancellationToken cancellationToken = default);
    Task<InteractionVM> AddInteractionAsync(Guid entityId, string entityType, string interactionType, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> RemoveInteractionAsync(Guid interactionId, CancellationToken cancellationToken = default);
    Task<InteractionStatsVM> GetInteractionStatsAsync(Guid entityId, string entityType, CancellationToken cancellationToken = default);
    Task<List<InteractionVM>> GetUserInteractionsAsync(Guid userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}