using System.Data;
using Microsoft.Data.SqlClient;

namespace Dapper.Api.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection SetOptionModels(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        return serviceCollection;
    }

    public static IServiceCollection SetServiceLifeCycles(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddSingleton<IDbConnection>(new SqlConnection(configuration.GetConnectionString("SQLServer")));
        return serviceCollection;
    }
}