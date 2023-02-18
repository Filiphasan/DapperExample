using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Dapper.Core.Data.Interfaces;
using Dapper.Core.Exceptions;

namespace Dapper.Data.Helpers;

public static class DbPropertyHelper
{
    public static string GetPrimaryKeyPropertyName<TEntity>() where TEntity: class, IDbEntity, new()
    {
        var type = typeof(TEntity);
        var primaryKeyProperty = type.GetProperties().FirstOrDefault(x => x.IsDefined(typeof(KeyAttribute)));
        if (primaryKeyProperty is null) throw new NoPrimaryKeyException($"{nameof(TEntity)} doesn't have Primary Key Property or missing [Key] attribute.");
        return primaryKeyProperty.Name;
    }
    
    public static string GetPrimaryKeyColumnName<TEntity>() where TEntity: class, IDbEntity, new()
    {
        var primaryKeyPropertyName = GetPrimaryKeyPropertyName<TEntity>();
        return GetDatabaseColumnName<TEntity>(primaryKeyPropertyName);
    }

    public static object GetPrimaryKeyColumnValue<TEntity>(TEntity entity) where TEntity: class, IDbEntity, new()
    {
        var primaryKeyPropertyName = GetPrimaryKeyPropertyName<TEntity>();
        var value = entity.GetType().GetProperties().First(x => x.Name == primaryKeyPropertyName).GetValue(entity);
        if (value is null) throw new NoPrimaryKeyValueException($"{nameof(TEntity)} doesn't have Primary Key Value.");
        return value;
    }

    public static string GetDatabaseColumnName<TEntity>(string propertyName) where TEntity: class, IDbEntity, new()
    {
        var type = typeof(TEntity);
        var property = type.GetProperty(propertyName);
        if (property is null) throw new NoDatabaseColumnException($"'{propertyName}' property doesn't exist in {nameof(TEntity)}");
        var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
        return columnAttribute?.Name ?? property.Name;
    }
}