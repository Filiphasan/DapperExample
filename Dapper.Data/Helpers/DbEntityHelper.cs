using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Reflection;
using Dapper.Core.Data.Interfaces;

namespace Dapper.Data.Helpers;

public static class DbEntityHelper
{
    public static string GetTableName<TEntity>() where TEntity : class, IDbEntity, new()
    {
        var type = typeof(TEntity);
        var tableAttribute = type.GetCustomAttribute<TableAttribute>();
        return tableAttribute?.Name ?? nameof(TEntity);
    }
    
}