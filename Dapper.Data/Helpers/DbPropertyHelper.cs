using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Dapper.Core.Data.Interfaces;
using Dapper.Core.Exceptions;

namespace Dapper.Data.Helpers;

public static class DbPropertyHelper
{
    public static string GetPrimaryKeyPropertyName<TEntity>() where TEntity : class, IDbEntity, new()
    {
        return GetPrimaryKeyPropertyInfo<TEntity>().Name;
    }

    public static string GetPrimaryKeyColumnName<TEntity>() where TEntity : class, IDbEntity, new()
    {
        var primaryKeyPropertyName = GetPrimaryKeyPropertyName<TEntity>();
        return GetDatabaseColumnName<TEntity>(primaryKeyPropertyName);
    }

    public static object GetPrimaryKeyColumnValue<TEntity>(TEntity entity) where TEntity : class, IDbEntity, new()
    {
        var primaryKeyPropertyName = GetPrimaryKeyPropertyName<TEntity>();
        var value = entity.GetType().GetProperties().First(x => x.Name == primaryKeyPropertyName).GetValue(entity);
        if (value is null) throw new NoPrimaryKeyValueException($"{nameof(TEntity)} doesn't have Primary Key Value.");
        return value;
    }

    public static string GetIdentityPropertyName<TEntity>() where TEntity : class, IDbEntity, new()
    {
        var type = typeof(TEntity);
        var identityProperty = type.GetProperties().FirstOrDefault(x => x.IsDefined(typeof(DatabaseGeneratedAttribute)));
        if (identityProperty is null)
        {
            var primaryKeyPropertyInfo = GetPrimaryKeyPropertyInfo<TEntity>();
            var numericTypes = new[]
            {
                typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
                typeof(int), typeof(uint), typeof(long), typeof(ulong)
            };
            if (numericTypes.Contains(primaryKeyPropertyInfo.PropertyType))
            {
                return primaryKeyPropertyInfo.Name;
            }
        }

        return identityProperty?.Name ?? string.Empty;
    }

    public static string GetDatabaseColumnName<TEntity>(string propertyName) where TEntity : class, IDbEntity, new()
    {
        var type = typeof(TEntity);
        var property = type.GetProperty(propertyName);
        if (property is null)
            throw new NoDatabaseColumnException($"'{propertyName}' property doesn't exist in {nameof(TEntity)}");
        var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
        return columnAttribute?.Name ?? property.Name;
    }

    private static PropertyInfo GetPrimaryKeyPropertyInfo<TEntity>() where TEntity : class, IDbEntity, new()
    {
        var type = typeof(TEntity);
        var primaryKeyProperty = type.GetProperties().FirstOrDefault(x => x.IsDefined(typeof(KeyAttribute)));
        if (primaryKeyProperty is null)
            throw new NoPrimaryKeyException(
                $"{nameof(TEntity)} doesn't have Primary Key Property or missing [Key] attribute.");
        return primaryKeyProperty;
    }
}