using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace CommunityCar.Application.Common.Extensions;

/// <summary>
/// Extensions for IServiceCollection to support application service registration
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all services implementing a specific interface from an assembly
    /// </summary>
    public static IServiceCollection AddServicesFromAssembly<TInterface>(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var interfaceType = typeof(TInterface);
        var implementations = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t))
            .ToList();

        foreach (var implementation in implementations)
        {
            var interfaces = implementation.GetInterfaces()
                .Where(i => i != interfaceType && interfaceType.IsAssignableFrom(i))
                .ToList();

            if (interfaces.Any())
            {
                foreach (var @interface in interfaces)
                {
                    services.Add(new ServiceDescriptor(@interface, implementation, lifetime));
                }
            }
            else
            {
                services.Add(new ServiceDescriptor(interfaceType, implementation, lifetime));
            }
        }

        return services;
    }

    /// <summary>
    /// Registers services by naming convention (Service suffix)
    /// </summary>
    public static IServiceCollection AddServicesByConvention(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var serviceTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"))
            .ToList();

        foreach (var serviceType in serviceTypes)
        {
            var interfaceName = $"I{serviceType.Name}";
            var interfaceType = assembly.GetTypes()
                .FirstOrDefault(t => t.IsInterface && t.Name == interfaceName);

            if (interfaceType != null)
            {
                services.Add(new ServiceDescriptor(interfaceType, serviceType, lifetime));
            }
        }

        return services;
    }

    /// <summary>
    /// Adds configuration section as strongly typed options
    /// </summary>
    public static IServiceCollection AddConfigurationOptions<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where T : class, new()
    {
        services.Configure<T>(configuration.GetSection(sectionName));
        return services;
    }

    /// <summary>
    /// Adds multiple configuration sections as options
    /// </summary>
    public static IServiceCollection AddConfigurationOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        params (Type OptionsType, string SectionName)[] options)
    {
        foreach (var (optionsType, sectionName) in options)
        {
            var configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethods()
                .First(m => m.Name == "Configure" && m.GetParameters().Length == 2)
                .MakeGenericMethod(optionsType);

            configureMethod.Invoke(null, new object[] { services, configuration.GetSection(sectionName) });
        }

        return services;
    }

    /// <summary>
    /// Conditionally adds a service
    /// </summary>
    public static IServiceCollection AddIf<TService, TImplementation>(
        this IServiceCollection services,
        bool condition,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TService : class
        where TImplementation : class, TService
    {
        if (condition)
        {
            services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
        }

        return services;
    }

    /// <summary>
    /// Adds a service with factory if condition is met
    /// </summary>
    public static IServiceCollection AddIf<TService>(
        this IServiceCollection services,
        bool condition,
        Func<IServiceProvider, TService> factory,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TService : class
    {
        if (condition)
        {
            services.Add(new ServiceDescriptor(typeof(TService), factory, lifetime));
        }

        return services;
    }

    /// <summary>
    /// Replaces an existing service registration
    /// </summary>
    public static IServiceCollection Replace<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TService : class
        where TImplementation : class, TService
    {
        var existingService = services.FirstOrDefault(s => s.ServiceType == typeof(TService));
        if (existingService != null)
        {
            services.Remove(existingService);
        }

        services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
        return services;
    }

    /// <summary>
    /// Adds a service only if it hasn't been registered yet
    /// </summary>
    public static IServiceCollection TryAdd<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TService : class
        where TImplementation : class, TService
    {
        if (!services.Any(s => s.ServiceType == typeof(TService)))
        {
            services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
        }

        return services;
    }

    /// <summary>
    /// Adds multiple implementations of the same interface
    /// </summary>
    public static IServiceCollection AddMultiple<TService>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        params Type[] implementations)
        where TService : class
    {
        foreach (var implementation in implementations)
        {
            if (typeof(TService).IsAssignableFrom(implementation))
            {
                services.Add(new ServiceDescriptor(typeof(TService), implementation, lifetime));
            }
        }

        return services;
    }
}