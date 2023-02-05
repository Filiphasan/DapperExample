namespace Dapper.Api.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection SetOptionModels(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        return serviceCollection;
    }
}