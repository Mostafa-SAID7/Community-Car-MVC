using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Application.Common.Interfaces.Repositories.Chat;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Common.Interfaces.Repositories.AI;
using CommunityCar.Application.Common.Interfaces.Repositories.Localization;

namespace CommunityCar.Application.Common.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IConversationRepository Conversations { get; }
    IMessageRepository Messages { get; }
    IQARepository QA { get; }
    IVoteRepository Votes { get; }
    IViewRepository Views { get; }
    IBookmarkRepository Bookmarks { get; }
    IReactionRepository Reactions { get; }
    ICommentRepository Comments { get; }
    IShareRepository Shares { get; }
    ICategoryRepository Categories { get; }
    ITagRepository Tags { get; }
    IPointOfInterestRepository PointsOfInterest { get; }
    ICheckInRepository CheckIns { get; }
    IRouteRepository Routes { get; }
    INewsRepository News { get; }
    IReviewsRepository Reviews { get; }
    IStoriesRepository Stories { get; }
    IRatingRepository Ratings { get; }
    IEventsRepository Events { get; }
    IGroupsRepository Groups { get; }
    IPostsRepository Posts { get; }
    IGuidesRepository Guides { get; }
    ILocalizationCultureRepository LocalizationCultures { get; }
    ILocalizationResourceRepository LocalizationResources { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}


