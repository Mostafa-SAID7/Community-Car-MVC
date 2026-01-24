using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Data;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Common.Interfaces.Repositories.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Common.Interfaces.Services.Storage;
using CommunityCar.Domain.Entities.Auth;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using CommunityCar.Infrastructure.Persistence.Repositories.Community;
using CommunityCar.Infrastructure.Persistence.Repositories.Identity;
using CommunityCar.Infrastructure.Persistence.Repositories.Shared;
using CommunityCar.Infrastructure.Persistence.UnitOfWork;
using CommunityCar.Infrastructure.Services.Authentication;
using CommunityCar.Infrastructure.Services.Communication;
using CommunityCar.Infrastructure.Services.Community;
using CommunityCar.Infrastructure.Services.Identity;
using CommunityCar.Infrastructure.Services.Storage;
using CommunityCar.Infrastructure.Services.Dashboard;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Services.Communication;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CommunityCar.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                    .EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddIdentity<User, IdentityRole<Guid>>(options => {
            // Password policy
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredUniqueChars = 4;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

            // Email confirmation
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedAccount = false;

            // Token settings
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IQARepository, QARepository>();
        services.AddScoped<IVoteRepository, VoteRepository>();
        services.AddScoped<IViewRepository, ViewRepository>();
        services.AddScoped<IBookmarkRepository, BookmarkRepository>();
        services.AddScoped<IReactionRepository, ReactionRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IShareRepository, ShareRepository>();
        services.AddScoped<IPointOfInterestRepository, PointOfInterestRepository>();
        services.AddScoped<ICheckInRepository, CheckInRepository>();
        services.AddScoped<IRouteRepository, RouteRepository>();
        services.AddScoped<INewsRepository, NewsRepository>();
        services.AddScoped<IReviewsRepository, ReviewsRepository>();
        services.AddScoped<IStoriesRepository, StoriesRepository>();
        services.AddScoped<IRatingRepository, RatingRepository>();
        services.AddScoped<IEventsRepository, EventsRepository>();
        services.AddScoped<IGroupsRepository, GroupsRepository>();
        services.AddScoped<IPostsRepository, PostsRepository>();
        services.AddScoped<IFriendsRepository, FriendsRepository>();
        services.AddScoped<IGuidesRepository, GuidesRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Authentication & Authorization services
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IGuidesNotificationService, GuidesNotificationService>();
        services.AddScoped<INewsNotificationService, NewsNotificationService>();

        // Storage services
        services.AddScoped<IFileStorageService, FileStorageService>();

        // System Services
        services.AddMemoryCache();
        services.AddScoped<IMaintenanceService, MaintenanceService>();

        // Add Authentication (Cookie only)
        services.AddAuthentication();

        // Configure cookie authentication
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Login";
            options.LogoutPath = "/Logout";
            options.AccessDeniedPath = "/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

        // Seeding
        services.AddScoped<CommunityCar.Infrastructure.Persistence.Seeding.DataSeeder>();

        return services;
    }
}
