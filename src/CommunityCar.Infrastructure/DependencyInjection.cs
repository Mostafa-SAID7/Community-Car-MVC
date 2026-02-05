using CommunityCar.Infrastructure.Configuration;
using CommunityCar.Infrastructure.Configuration.Account;
using CommunityCar.Web.Areas.Identity.Interfaces.Services;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Shared;
using CommunityCar.Application.Common.Interfaces.Data;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Application.Common.Interfaces.Repositories.AI;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Common.Interfaces.Repositories.Localization;
using CommunityCar.Application.Common.Interfaces.Authentication;
using CommunityCar.Application.Services.Shared;
using CommunityCar.Application.Services.AI;
using CommunityCar.Application.Services.AI.ModelManagement;
using CommunityCar.Application.Services.AI.Training;
using CommunityCar.Application.Services.AI.History;
using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Entities.Account.Authorization;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using CommunityCar.Infrastructure.Persistence.Repositories.Community;
using CommunityCar.Infrastructure.Persistence.Repositories.Shared;
using CommunityCar.Infrastructure.Persistence.Repositories.AI;
using CommunityCar.Infrastructure.Persistence.Repositories.Localization;
using CommunityCar.Infrastructure.Persistence.UnitOfWork;
using CommunityCar.Infrastructure.Hubs;
using CommunityCar.Application.Common.Interfaces.Hubs;
using CommunityCar.Application.Common.Interfaces.Services.Community.Broadcast;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Account settings
        services.AddAccountConfiguration(configuration);
        AccountConfigurationExtensions.ValidateAccountConfiguration(configuration);

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                    .EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddHttpContextAccessor();

        services.AddIdentity<User, CommunityCar.Domain.Entities.Account.Authorization.Role>(options => {
            // Password policy
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredUniqueChars = 4;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            options.Lockout.MaxFailedAccessAttempts = 999;
            options.Lockout.AllowedForNewUsers = false;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

            // Email confirmation
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedAccount = false;

            // Token settings
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/login");
            options.LogoutPath = new Microsoft.AspNetCore.Http.PathString("/logout");
            options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/access-denied");
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.SlidingExpiration = true;
        });

        // Configure external authentication providers
        services.AddAuthentication()
            .AddGoogle(options =>
            {
                var googleSettings = configuration.GetSection("SocialAuth:Google");
                options.ClientId = googleSettings["ClientId"] ?? "";
                options.ClientSecret = googleSettings["ClientSecret"] ?? "";
                options.SaveTokens = true;
                options.Scope.Add("profile");
                options.Scope.Add("email");
            })
            .AddFacebook(options =>
            {
                var facebookSettings = configuration.GetSection("SocialAuth:Facebook");
                options.AppId = facebookSettings["AppId"] ?? "";
                options.AppSecret = facebookSettings["AppSecret"] ?? "";
                options.SaveTokens = true;
                options.Scope.Add("email");
                options.Scope.Add("public_profile");
            });

        // Community Repositories
        services.AddScoped<IQARepository, QARepository>();
        services.AddScoped<IVoteRepository, VoteRepository>();
        services.AddScoped<IViewRepository, ViewRepository>();
        services.AddScoped<IBookmarkRepository, BookmarkRepository>();
        services.AddScoped<IReactionRepository, ReactionRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IShareRepository, ShareRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
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

        // AI Repositories
        services.AddScoped<IAIModelRepository, AIModelRepository>();
        services.AddScoped<ITrainingJobRepository, TrainingJobRepository>();
        services.AddScoped<ITrainingHistoryRepository, TrainingHistoryRepository>();

        // AI Services
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Localization Repositories
        services.AddScoped<ILocalizationCultureRepository, LocalizationCultureRepository>();
        services.AddScoped<ILocalizationResourceRepository, LocalizationResourceRepository>();

        // SignalR Hub Context Wrapper
        services.AddScoped<INotificationHubContext, NotificationHubContextWrapper>();
        
        // System Services
        services.AddMemoryCache();
        services.AddHttpClient();

        // Seeding
        services.AddScoped<CommunityCar.Infrastructure.Persistence.Seeding.DataSeeder>(provider =>
            new CommunityCar.Infrastructure.Persistence.Seeding.DataSeeder(
                provider.GetRequiredService<ApplicationDbContext>(),
                provider.GetRequiredService<UserManager<User>>(),
                provider.GetRequiredService<RoleManager<CommunityCar.Domain.Entities.Account.Authorization.Role>>(),
                provider.GetRequiredService<ILogger<CommunityCar.Infrastructure.Persistence.Seeding.DataSeeder>>(),
                provider));

        return services;
    }
}

