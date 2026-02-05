using CommunityCar.Web.Areas.Communication.Interfaces.Repositories;
using CommunityCar.Web.Areas.Communication.Interfaces.Repositories.Chat;
using CommunityCar.Web.Areas.Communication.Interfaces.Services;
using CommunityCar.Web.Areas.Communication.Interfaces.Services.Chat;
using CommunityCar.Web.Areas.Communication.Interfaces.Services.Email;
using CommunityCar.Web.Areas.Communication.Interfaces.Services.Notifications;
using CommunityCar.Web.Areas.Communication.Repositories;
using CommunityCar.Web.Areas.Communication.Repositories.Chat;
using CommunityCar.Web.Areas.Communication.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityCar.Web.Areas.Communication.Configuration;

public static class CommunicationDependencyInjection
{
    public static IServiceCollection AddCommunicationAreaServices(this IServiceCollection services)
    {
        // Unit of Work
        services.AddScoped<ICommunicationUnitOfWork, CommunicationUnitOfWork>();

        // Repositories
        services.AddScoped<IConversationParticipantRepository, ConversationParticipantRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        // Services
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}
