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

    public static string CreateInsertSqlQueryStringDapper<TEntity>(string tableName)
        where TEntity : class, IDbEntity, new()
    {
        if (string.IsNullOrWhiteSpace(tableName)) tableName = GetTableName<TEntity>();
        List<string> dbColumnNameList = new();
        List<string> dbColumnValueList = new();
        var properties = typeof(TEntity).GetProperties().ToList();
        foreach (var propertyInfo in properties.Select(x => x.Name))
        {
            string dbColumnName = DbPropertyHelper.GetDatabaseColumnName<TEntity>(propertyInfo);
            dbColumnNameList.Add(dbColumnName);
            dbColumnValueList.Add($"@{propertyInfo}");
        }

        return $"INSERT INTO {tableName} ({string.Join(", ", dbColumnNameList)}) VALUES ({string.Join(", ", dbColumnValueList)})";
    }
    
    public static string CreateUpdateAllColumnSqlQueryStringDapper<TEntity>(TEntity entity ,string tableName)
        where TEntity : class, IDbEntity, new()
    {
        if (string.IsNullOrWhiteSpace(tableName)) tableName = GetTableName<TEntity>();
        List<string> list = new();
        var properties = typeof(TEntity).GetProperties().ToList();
        foreach (var propertyInfo in properties.Select(x => x.Name))
        {
            string dbColumnName = DbPropertyHelper.GetDatabaseColumnName<TEntity>(propertyInfo);
            list.Add($"{dbColumnName}=@{propertyInfo}");
        }

        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        var pkColumnValue = properties.First(x => x.Name == DbPropertyHelper.GetPrimaryKeyPropertyName<TEntity>()).GetValue(entity);
        return $"UPDATE {tableName} SET ({string.Join(", ", list)}) WHERE {pkColumnName}={pkColumnValue}";
    }
    
    public static string CreateUpdateSpecificColumnSqlQueryStringDapper<TEntity>(ExpandoObject fields, object id, string tableName)
        where TEntity : class, IDbEntity, new()
    {
        if (string.IsNullOrWhiteSpace(tableName)) tableName = GetTableName<TEntity>();
        List<string> list = new();
        var properties = fields.GetType().GetProperties().ToList();
        foreach (var propertyInfo in properties.Select(x => x.Name))
        {
            string dbColumnName = DbPropertyHelper.GetDatabaseColumnName<TEntity>(propertyInfo);
            list.Add($"{dbColumnName}=@{propertyInfo}");
        }

        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        return $"UPDATE {tableName} SET ({string.Join(", ", list)}) WHERE {pkColumnName}={id}";
    }
}