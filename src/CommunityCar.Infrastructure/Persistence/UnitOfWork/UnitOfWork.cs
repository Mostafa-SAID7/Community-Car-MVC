using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Common.Interfaces.Repositories.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Infrastructure.Persistence.Data;

namespace CommunityCar.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(
        ApplicationDbContext context,
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        IQARepository qaRepository,
        IVoteRepository voteRepository,
        IViewRepository viewRepository,
        IBookmarkRepository bookmarkRepository,
        IReactionRepository reactionRepository,
        ICommentRepository commentRepository,
        IShareRepository shareRepository,
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository,
        IPointOfInterestRepository pointOfInterestRepository,
        ICheckInRepository checkInRepository,
        IRouteRepository routeRepository,
        INewsRepository newsRepository,
        IReviewsRepository reviewsRepository,
        IStoriesRepository storiesRepository,
        IRatingRepository ratingRepository,
        IEventsRepository eventsRepository,
        IGroupsRepository groupsRepository,
        IPostsRepository postsRepository,
        IGuidesRepository guidesRepository,
        IUserRepository userRepository)
    {
        _context = context;
        Conversations = conversationRepository;
        Messages = messageRepository;
        QA = qaRepository;
        Votes = voteRepository;
        Views = viewRepository;
        Bookmarks = bookmarkRepository;
        Reactions = reactionRepository;
        Comments = commentRepository;
        Shares = shareRepository;
        Categories = categoryRepository;
        Tags = tagRepository;
        PointsOfInterest = pointOfInterestRepository;
        CheckIns = checkInRepository;
        Routes = routeRepository;
        News = newsRepository;
        Reviews = reviewsRepository;
        Stories = storiesRepository;
        Ratings = ratingRepository;
        Events = eventsRepository;
        Groups = groupsRepository;
        Posts = postsRepository;
        Guides = guidesRepository;
        Users = userRepository;
    }

    public IConversationRepository Conversations { get; }
    public IMessageRepository Messages { get; }
    public IQARepository QA { get; }
    public IVoteRepository Votes { get; }
    public IViewRepository Views { get; }
    public IBookmarkRepository Bookmarks { get; }
    public IReactionRepository Reactions { get; }
    public ICommentRepository Comments { get; }
    public IShareRepository Shares { get; }
    public ICategoryRepository Categories { get; }
    public ITagRepository Tags { get; }
    public IPointOfInterestRepository PointsOfInterest { get; }
    public ICheckInRepository CheckIns { get; }
    public IRouteRepository Routes { get; }
    public INewsRepository News { get; }
    public IReviewsRepository Reviews { get; }
    public IStoriesRepository Stories { get; }
    public IRatingRepository Ratings { get; }
    public IEventsRepository Events { get; }
    public IGroupsRepository Groups { get; }
    public IPostsRepository Posts { get; }
    public IGuidesRepository Guides { get; }
    public IUserRepository Users { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}