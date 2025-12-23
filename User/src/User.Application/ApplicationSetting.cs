using Microsoft.Extensions.DependencyInjection;

namespace User.Application;

public static class ApplicationSetting
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationSetting).Assembly;
        services.AddMediatR(x => 
            x.RegisterServicesFromAssembly(assembly));
        return services;
    }
}