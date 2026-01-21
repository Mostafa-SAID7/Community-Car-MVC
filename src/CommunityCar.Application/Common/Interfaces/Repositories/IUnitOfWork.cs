using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Common.Interfaces.Repositories.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;

namespace CommunityCar.Application.Common.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IQARepository QA { get; }
    IVoteRepository Votes { get; }
    IViewRepository Views { get; }
    IBookmarkRepository Bookmarks { get; }
    IReactionRepository Reactions { get; }
    ICommentRepository Comments { get; }
    IShareRepository Shares { get; }
    IPointOfInterestRepository PointsOfInterest { get; }
    ICheckInRepository CheckIns { get; }
    IRouteRepository Routes { get; }
    INewsRepository News { get; }
    IReviewsRepository Reviews { get; }
    IStoriesRepository Stories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}