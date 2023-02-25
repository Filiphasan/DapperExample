using System.Data;
using Dapper.Core.Services;
using Dapper.Core.Services.interfaces;
using Dapper.Data.Implementations;
using Dapper.Data.Interfaces;
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
        serviceCollection.AddSingleton<IHashService, HashService>();
        serviceCollection.AddSingleton<IDbConnection>(new SqlConnection(configuration.GetConnectionString("SQLServer")));
        serviceCollection.AddTransient(typeof(IDapperRepository<>), typeof(DapperRepository<>));
        serviceCollection.AddTransient<IUserRepository, UserRepository>();
        return serviceCollection;
    }
}