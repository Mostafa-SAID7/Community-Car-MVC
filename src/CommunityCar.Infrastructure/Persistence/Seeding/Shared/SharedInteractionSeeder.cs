using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Entities.Account;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Shared;

public class SharedInteractionSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SharedInteractionSeeder> _logger;

    public SharedInteractionSeeder(ApplicationDbContext context, ILogger<SharedInteractionSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Seeding shared interactions...");

            var users = await _context.Users.Take(50).ToListAsync();
            var posts = await _context.Posts.Take(20).ToListAsync();
            var questions = await _context.Questions.Take(15).ToListAsync();
            var stories = await _context.Stories.Take(25).ToListAsync();
            var reviews = await _context.Reviews.Take(10).ToListAsync();
            var guides = await _context.Guides.Take(12).ToListAsync();
            var news = await _context.News.Take(8).ToListAsync();

            if (!users.Any())
            {
                _logger.LogWarning("No users found for seeding interactions");
                return;
            }

            var random = new Random();

            // Seed each interaction type independently
            await SeedReactionsIfNeededAsync(users, posts.Cast<object>().ToList(), questions.Cast<object>().ToList(), 
                stories.Cast<object>().ToList(), reviews.Cast<object>().ToList(), guides.Cast<object>().ToList(), 
                news.Cast<object>().ToList(), random);
            await SeedCommentsIfNeededAsync(users, posts.Cast<object>().ToList(), questions.Cast<object>().ToList(), 
                stories.Cast<object>().ToList(), reviews.Cast<object>().ToList(), guides.Cast<object>().ToList(), 
                news.Cast<object>().ToList(), random);
            await SeedVotesIfNeededAsync(users, questions.Cast<object>().ToList(), random);
            await SeedSharesIfNeededAsync(users, posts.Cast<object>().ToList(), stories.Cast<object>().ToList(), 
                news.Cast<object>().ToList(), random);
            await SeedRatingsIfNeededAsync(users, reviews.Cast<object>().ToList(), guides.Cast<object>().ToList(), random);
            await SeedBookmarksIfNeededAsync(users, posts.Cast<object>().ToList(), questions.Cast<object>().ToList(), 
                guides.Cast<object>().ToList(), reviews.Cast<object>().ToList(), random);
            await SeedViewsIfNeededAsync(users, posts.Cast<object>().ToList(), questions.Cast<object>().ToList(), 
                stories.Cast<object>().ToList(), news.Cast<object>().ToList(), random);

            _logger.LogInformation("Successfully seeded shared interactions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding shared interactions");
            throw;
        }
    }

    private async Task SeedReactionsIfNeededAsync(List<User> users, List<object> posts, List<object> questions, 
        List<object> stories, List<object> reviews, List<object> guides, List<object> news, Random random)
    {
        if (await _context.Reactions.AnyAsync())
        {
            _logger.LogInformation("Reactions already exist, skipping reactions seeding");
            return;
        }

        var reactions = new List<Reaction>();

        // Seed reactions for different entity types
        await SeedReactionsAsync(users, posts, EntityType.Post, reactions, random);
        await SeedReactionsAsync(users, questions, EntityType.Question, reactions, random);
        await SeedReactionsAsync(users, stories, EntityType.Story, reactions, random);
        await SeedReactionsAsync(users, reviews, EntityType.Review, reactions, random);
        await SeedReactionsAsync(users, guides, EntityType.Guide, reactions, random);
        await SeedReactionsAsync(users, news, EntityType.News, reactions, random);

        if (reactions.Any())
        {
            await _context.Reactions.AddRangeAsync(reactions);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added {Count} reactions", reactions.Count);
        }
    }

    private async Task SeedCommentsIfNeededAsync(List<User> users, List<object> posts, List<object> questions, 
        List<object> stories, List<object> reviews, List<object> guides, List<object> news, Random random)
    {
        if (await _context.Comments.AnyAsync())
        {
            _logger.LogInformation("Comments already exist, skipping comments seeding");
            return;
        }

        var comments = new List<Comment>();

        // Seed comments for different entity types
        await SeedCommentsAsync(users, posts, EntityType.Post, comments, random);
        await SeedCommentsAsync(users, questions, EntityType.Question, comments, random);
        await SeedCommentsAsync(users, stories, EntityType.Story, comments, random);
        await SeedCommentsAsync(users, reviews, EntityType.Review, comments, random);
        await SeedCommentsAsync(users, guides, EntityType.Guide, comments, random);
        await SeedCommentsAsync(users, news, EntityType.News, comments, random);

        if (comments.Any())
        {
            await _context.Comments.AddRangeAsync(comments);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added {Count} comments", comments.Count);
        }
    }

    private async Task SeedVotesIfNeededAsync(List<User> users, List<object> questions, Random random)
    {
        if (await _context.Votes.AnyAsync())
        {
            _logger.LogInformation("Votes already exist, skipping votes seeding");
            return;
        }

        var votes = new List<Vote>();

        // Seed votes (mainly for Q&A)
        await SeedVotesAsync(users, questions, EntityType.Question, votes, random);
        if (await _context.Answers.AnyAsync())
        {
            var answers = await _context.Answers.Take(30).ToListAsync();
            await SeedVotesAsync(users, answers.Cast<object>().ToList(), EntityType.Answer, votes, random);
        }

        if (votes.Any())
        {
            await _context.Votes.AddRangeAsync(votes);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added {Count} votes", votes.Count);
        }
    }

    private async Task SeedSharesIfNeededAsync(List<User> users, List<object> posts, List<object> stories, List<object> news, Random random)
    {
        if (await _context.Shares.AnyAsync())
        {
            _logger.LogInformation("Shares already exist, skipping shares seeding");
            return;
        }

        var shares = new List<Share>();

        // Seed shares
        await SeedSharesAsync(users, posts, EntityType.Post, shares, random);
        await SeedSharesAsync(users, stories, EntityType.Story, shares, random);
        await SeedSharesAsync(users, news, EntityType.News, shares, random);

        if (shares.Any())
        {
            await _context.Shares.AddRangeAsync(shares);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added {Count} shares", shares.Count);
        }
    }

    private async Task SeedRatingsIfNeededAsync(List<User> users, List<object> reviews, List<object> guides, Random random)
    {
        if (await _context.Ratings.AnyAsync())
        {
            _logger.LogInformation("Ratings already exist, skipping ratings seeding");
            return;
        }

        var ratings = new List<Rating>();

        // Seed ratings (for reviews, guides, etc.)
        await SeedRatingsAsync(users, reviews, EntityType.Review, ratings, random);
        await SeedRatingsAsync(users, guides, EntityType.Guide, ratings, random);

        if (ratings.Any())
        {
            await _context.Ratings.AddRangeAsync(ratings);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added {Count} ratings", ratings.Count);
        }
    }

    private async Task SeedBookmarksIfNeededAsync(List<User> users, List<object> posts, List<object> questions, 
        List<object> guides, List<object> reviews, Random random)
    {
        if (await _context.Bookmarks.AnyAsync())
        {
            _logger.LogInformation("Bookmarks already exist, skipping bookmarks seeding");
            return;
        }

        var bookmarks = new List<Bookmark>();

        // Seed bookmarks
        await SeedBookmarksAsync(users, posts, EntityType.Post, bookmarks, random);
        await SeedBookmarksAsync(users, questions, EntityType.Question, bookmarks, random);
        await SeedBookmarksAsync(users, guides, EntityType.Guide, bookmarks, random);
        await SeedBookmarksAsync(users, reviews, EntityType.Review, bookmarks, random);

        if (bookmarks.Any())
        {
            await _context.Bookmarks.AddRangeAsync(bookmarks);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added {Count} bookmarks", bookmarks.Count);
        }
    }

    private async Task SeedViewsIfNeededAsync(List<User> users, List<object> posts, List<object> questions, 
        List<object> stories, List<object> news, Random random)
    {
        if (await _context.Views.AnyAsync())
        {
            _logger.LogInformation("Views already exist, skipping views seeding");
            return;
        }

        var views = new List<View>();

        // Seed views
        await SeedViewsAsync(users, posts, EntityType.Post, views, random);
        await SeedViewsAsync(users, questions, EntityType.Question, views, random);
        await SeedViewsAsync(users, stories, EntityType.Story, views, random);
        await SeedViewsAsync(users, news, EntityType.News, views, random);

        if (views.Any())
        {
            await _context.Views.AddRangeAsync(views);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added {Count} views", views.Count);
        }
    }

    private async Task SeedReactionsAsync<T>(List<User> users, List<T> entities, EntityType entityType, List<Reaction> reactions, Random random) where T : class
    {
        if (!entities.Any()) return;

        var reactionTypes = Enum.GetValues<ReactionType>();
        
        foreach (var entity in entities)
        {
            var entityId = GetEntityId(entity);
            var reactionCount = random.Next(0, Math.Min(users.Count / 2, 25)); // Up to 25 reactions per entity
            var selectedUsers = users.OrderBy(x => random.Next()).Take(reactionCount).ToList();

            foreach (var user in selectedUsers)
            {
                var reactionType = reactionTypes[random.Next(reactionTypes.Length)];
                reactions.Add(new Reaction(entityId, entityType, user.Id, reactionType));
            }
        }
    }

    private async Task SeedCommentsAsync<T>(List<User> users, List<T> entities, EntityType entityType, List<Comment> comments, Random random) where T : class
    {
        if (!entities.Any()) return;

        var commentTexts = new[]
        {
            "Great post! Thanks for sharing.",
            "I had a similar experience with my car.",
            "This is really helpful information.",
            "Could you provide more details about this?",
            "I disagree with this approach, here's why...",
            "Excellent write-up! Very thorough.",
            "This saved me a lot of time and money.",
            "Has anyone else tried this method?",
            "I'm having the same issue. Any updates?",
            "Thanks for the detailed explanation.",
            "This is exactly what I was looking for!",
            "Great photos! What camera did you use?",
            "I've been doing this for years and can confirm it works.",
            "Be careful with this approach, it can cause damage.",
            "What tools do you recommend for this job?",
            "How long did this take you to complete?",
            "The results look amazing! Well done.",
            "I tried this and it didn't work for me.",
            "This is a common problem with this model.",
            "You should also check the warranty before doing this."
        };

        foreach (var entity in entities)
        {
            var entityId = GetEntityId(entity);
            var commentCount = random.Next(0, 8); // Up to 8 comments per entity
            var selectedUsers = users.OrderBy(x => random.Next()).Take(commentCount).ToList();

            var entityComments = new List<Comment>();

            foreach (var user in selectedUsers)
            {
                var commentText = commentTexts[random.Next(commentTexts.Length)];
                var comment = new Comment(commentText, entityId, entityType, user.Id);
                entityComments.Add(comment);
                comments.Add(comment);
            }

            // Add some replies to comments (30% chance)
            foreach (var comment in entityComments.Where(c => random.NextDouble() < 0.3))
            {
                var replyCount = random.Next(1, 3);
                var replyUsers = users.Where(u => u.Id != comment.AuthorId).OrderBy(x => random.Next()).Take(replyCount).ToList();

                foreach (var replyUser in replyUsers)
                {
                    var replyText = commentTexts[random.Next(commentTexts.Length)];
                    var reply = new Comment(replyText, entityId, entityType, replyUser.Id, comment.Id);
                    comments.Add(reply);
                }
            }
        }
    }

    private async Task SeedVotesAsync<T>(List<User> users, List<T> entities, EntityType entityType, List<Vote> votes, Random random) where T : class
    {
        if (!entities.Any()) return;

        var voteTypes = Enum.GetValues<VoteType>();

        foreach (var entity in entities)
        {
            var entityId = GetEntityId(entity);
            var voteCount = random.Next(0, Math.Min(users.Count / 3, 15)); // Up to 15 votes per entity
            var selectedUsers = users.OrderBy(x => random.Next()).Take(voteCount).ToList();

            foreach (var user in selectedUsers)
            {
                // Bias towards upvotes (70% upvote, 30% downvote)
                var voteType = random.NextDouble() < 0.7 ? VoteType.Upvote : VoteType.Downvote;
                votes.Add(new Vote(entityId, entityType, user.Id, voteType));
            }
        }
    }

    private async Task SeedSharesAsync<T>(List<User> users, List<T> entities, EntityType entityType, List<Share> shares, Random random) where T : class
    {
        if (!entities.Any()) return;

        var shareTypes = Enum.GetValues<ShareType>();
        var platforms = new[] { "Facebook", "Twitter", "WhatsApp", "LinkedIn", "Reddit", "Email", "Copy Link" };
        var shareMessages = new[]
        {
            "Check this out!",
            "Thought you might find this interesting",
            "Great read about cars",
            "This is really helpful",
            "Must read for car enthusiasts",
            null, null, null // Some shares without messages
        };

        foreach (var entity in entities)
        {
            var entityId = GetEntityId(entity);
            var shareCount = random.Next(0, 5); // Up to 5 shares per entity
            var selectedUsers = users.OrderBy(x => random.Next()).Take(shareCount).ToList();

            foreach (var user in selectedUsers)
            {
                var shareType = shareTypes[random.Next(shareTypes.Length)];
                var platform = platforms[random.Next(platforms.Length)];
                var message = shareMessages[random.Next(shareMessages.Length)];
                
                shares.Add(new Share(entityId, entityType, user.Id, shareType, message, platform));
            }
        }
    }

    private async Task SeedRatingsAsync<T>(List<User> users, List<T> entities, EntityType entityType, List<Rating> ratings, Random random) where T : class
    {
        if (!entities.Any()) return;

        var reviewTexts = new[]
        {
            "Excellent quality and very helpful!",
            "Good information, but could be more detailed.",
            "Average content, nothing special.",
            "Very comprehensive and well-written.",
            "Not what I expected, but still useful.",
            "Outstanding work! Highly recommended.",
            "Decent content for beginners.",
            "Could use more examples and illustrations.",
            "Perfect for what I needed.",
            "Well-researched and informative.",
            null, null, null // Some ratings without reviews
        };

        foreach (var entity in entities)
        {
            var entityId = GetEntityId(entity);
            var ratingCount = random.Next(0, 10); // Up to 10 ratings per entity
            var selectedUsers = users.OrderBy(x => random.Next()).Take(ratingCount).ToList();

            foreach (var user in selectedUsers)
            {
                // Bias towards higher ratings (normal distribution around 4)
                var rating = Math.Max(1, Math.Min(5, (int)Math.Round(random.NextGaussian(4.0, 1.0))));
                var review = random.NextDouble() < 0.4 ? reviewTexts[random.Next(reviewTexts.Length)] : null;
                
                ratings.Add(new Rating(entityId, entityType, user.Id, rating, review));
            }
        }
    }

    private async Task SeedBookmarksAsync<T>(List<User> users, List<T> entities, EntityType entityType, List<Bookmark> bookmarks, Random random) where T : class
    {
        if (!entities.Any()) return;

        var bookmarkNotes = new[]
        {
            "Need to try this later",
            "Great reference material",
            "Bookmark for future project",
            "Useful troubleshooting guide",
            "Remember to show this to friends",
            "Good example for my car",
            "Save for weekend reading",
            null, null, null, null // Many bookmarks without notes
        };

        foreach (var entity in entities)
        {
            var entityId = GetEntityId(entity);
            var bookmarkCount = random.Next(0, 8); // Up to 8 bookmarks per entity
            var selectedUsers = users.OrderBy(x => random.Next()).Take(bookmarkCount).ToList();

            foreach (var user in selectedUsers)
            {
                var note = bookmarkNotes[random.Next(bookmarkNotes.Length)];
                bookmarks.Add(new Bookmark(entityId, entityType, user.Id, note));
            }
        }
    }

    private async Task SeedViewsAsync<T>(List<User> users, List<T> entities, EntityType entityType, List<View> views, Random random) where T : class
    {
        if (!entities.Any()) return;

        var userAgents = new[]
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36",
            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 14_7_1 like Mac OS X) AppleWebKit/605.1.15",
            "Mozilla/5.0 (Android 11; Mobile; rv:68.0) Gecko/68.0 Firefox/88.0"
        };

        foreach (var entity in entities)
        {
            var entityId = GetEntityId(entity);
            var viewCount = random.Next(5, 50); // 5-50 views per entity

            for (int i = 0; i < viewCount; i++)
            {
                // 70% authenticated views, 30% anonymous
                Guid? userId = null;
                if (random.NextDouble() < 0.7)
                {
                    userId = users[random.Next(users.Count)].Id;
                }

                var ipAddress = $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}";
                var userAgent = userAgents[random.Next(userAgents.Length)];

                views.Add(new View(entityId, entityType, ipAddress, userAgent, userId));
            }
        }
    }

    private Guid GetEntityId<T>(T entity) where T : class
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null && idProperty.PropertyType == typeof(Guid))
        {
            return (Guid)idProperty.GetValue(entity)!;
        }
        throw new InvalidOperationException($"Entity type {typeof(T).Name} does not have a Guid Id property");
    }
}

// Extension method for Gaussian random numbers
public static class RandomExtensions
{
    public static double NextGaussian(this Random random, double mean = 0.0, double stdDev = 1.0)
    {
        // Box-Muller transform
        double u1 = 1.0 - random.NextDouble();
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + stdDev * randStdNormal;
    }
}
