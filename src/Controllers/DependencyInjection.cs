using Microsoft.Extensions.DependencyInjection;

namespace Controllers;

public static class DependencyInjection
{
    /// <summary>Registers MVC controllers defined in the Controllers assembly.</summary>
    public static IServiceCollection AddControllersLayer(this IServiceCollection services)
    {
        services.AddControllers()
            .AddApplicationPart(typeof(DependencyInjection).Assembly);
        return services;
    }
}
