using System.Dynamic;
using Dapper.Core.Data.Interfaces;

namespace Dapper.Data.Helpers;

internal static class DbSqlBuilderHelper
{
    internal static string GetByIdSqlString<TEntity>(string tableName, object id)
        where TEntity : class, IDbEntity, new()
    {
        string resultStr;
        var selectPartOfSqlStr = GetSelectPartSqlString<TEntity>();
        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        resultStr = string.Format("SELECT TOP 1 {0} FROM [{1}] WHERE {2} = {3}", selectPartOfSqlStr, tableName,
            pkColumnName, id);

        return resultStr;
    }

    internal static string GetByIdSqlString<TEntity, TResult>(string tableName, object id)
        where TEntity : class, IDbEntity, new() where TResult : class, new()
    {
        string resultStr;
        var selectPartOfSqlStr = GetSelectPartSqlString<TEntity, TResult>();
        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        resultStr = string.Format("SELECT TOP 1 {0} FROM [{1}] WHERE {2} = {3}", selectPartOfSqlStr, tableName,
            pkColumnName, id);

        return resultStr;
    }

    internal static string GetSqlString<TEntity>(string tableName, string whereConditionStr)
        where TEntity : class, IDbEntity, new()
    {
        string resultStr;
        var selectPartOfSqlStr = GetSelectPartSqlString<TEntity>();
        resultStr = string.Format("SELECT {0} FROM [{1}]", selectPartOfSqlStr, tableName);
        if (!string.IsNullOrWhiteSpace(whereConditionStr))
        {
            resultStr = $"{resultStr} WHERE {whereConditionStr}";
        }

        return resultStr;
    }

    internal static string GetSqlString<TEntity, TResult>(string tableName, string whereConditionStr)
        where TEntity : class, IDbEntity, new() where TResult : class, new()
    {
        string resultStr;
        var selectPartOfSqlStr = GetSelectPartSqlString<TEntity, TResult>();
        resultStr = string.Format("SELECT {0} FROM [{1}]", selectPartOfSqlStr, tableName);
        if (!string.IsNullOrWhiteSpace(whereConditionStr))
        {
            resultStr = $"{resultStr} WHERE {whereConditionStr}";
        }

        return resultStr;
    }

    internal static string GetFirstSqlString<TEntity>(string tableName, string whereConditionStr)
        where TEntity : class, IDbEntity, new()
    {
        string resultStr;
        var selectPartOfSqlStr = GetSelectPartSqlString<TEntity>();

        resultStr = string.Format("SELECT TOP 1 {0} FROM {1}", selectPartOfSqlStr, tableName);
        if (!string.IsNullOrWhiteSpace(whereConditionStr))
        {
            resultStr = $"{resultStr} WHERE {whereConditionStr}";
        }

        return resultStr;
    }

    internal static string GetFirstSqlString<TEntity, TResult>(string tableName, string whereConditionStr)
        where TEntity : class, IDbEntity, new()
        where TResult : class, new()
    {
        string resultStr;
        var selectPartOfSqlStr = GetSelectPartSqlString<TEntity, TResult>();

        resultStr = string.Format("SELECT TOP 1 {0} FROM {1}", selectPartOfSqlStr, tableName);
        if (!string.IsNullOrWhiteSpace(whereConditionStr))
        {
            resultStr = $"{resultStr} WHERE {whereConditionStr}";
        }

        return resultStr;
    }

    internal static string GetLastSqlString<TEntity>(string tableName, string whereConditionStr)
        where TEntity : class, IDbEntity, new()
    {
        var baseSqlStr = GetFirstSqlString<TEntity>(tableName, whereConditionStr);
        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        return $"{baseSqlStr} ORDER BY {pkColumnName} DESC";
    }
    
    internal static string GetLastSqlString<TEntity, TResult>(string tableName, string whereConditionStr)
        where TEntity : class, IDbEntity, new() where TResult : class, new()
    {
        var baseSqlStr = GetFirstSqlString<TEntity, TResult>(tableName, whereConditionStr);
        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        return $"{baseSqlStr} ORDER BY {pkColumnName} DESC";
    }

    internal static string GetDeleteSqlString<TEntity>(string tableName, object id)
        where TEntity : class, IDbEntity, new()
    {
        string resultStr;
        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        resultStr = string.Format("DELETE FROM [{0}] WHERE {1}={2}", tableName, pkColumnName, id);
        return resultStr;
    }

    internal static string GetAnySqlString(string tableName, string whereConditionStr)
    {
        string resultStr;
        resultStr = $"DECLARE @HAS_EXIST BIT=0; SELECT @HAS_EXIST=1 FROM [{tableName}]";
        if (!string.IsNullOrWhiteSpace(whereConditionStr))
        {
            resultStr = $"{resultStr} WHERE {whereConditionStr}";
        }

        return resultStr + " SELECT @HAS_EXIST;";
    }

    internal static string CreateInsertSqlQueryStringDapper<TEntity>(string tableName)
        where TEntity : class, IDbEntity, new()
    {
        if (string.IsNullOrWhiteSpace(tableName)) tableName = DbEntityHelper.GetTableName<TEntity>();
        List<string> dbColumnNameList = new();
        List<string> dbColumnValueList = new();
        var properties = typeof(TEntity).GetProperties().ToList();
        var identityPropertyName = DbPropertyHelper.GetIdentityPropertyName<TEntity>();
        foreach (var propertyInfo in properties.Select(x => x.Name))
        {
            if (propertyInfo.Equals(identityPropertyName)) continue;
            string dbColumnName = DbPropertyHelper.GetDatabaseColumnName<TEntity>(propertyInfo);
            dbColumnNameList.Add(dbColumnName);
            dbColumnValueList.Add($"@{propertyInfo}");
        }

        return
            $"INSERT INTO {tableName} ({string.Join(", ", dbColumnNameList)}) VALUES ({string.Join(", ", dbColumnValueList)})";
    }

    internal static string CreateUpdateAllColumnSqlQueryStringDapper<TEntity>(TEntity entity, string tableName)
        where TEntity : class, IDbEntity, new()
    {
        if (string.IsNullOrWhiteSpace(tableName)) tableName = DbEntityHelper.GetTableName<TEntity>();
        List<string> list = new();
        var properties = typeof(TEntity).GetProperties().ToList();
        foreach (var propertyInfo in properties.Select(x => x.Name))
        {
            string dbColumnName = DbPropertyHelper.GetDatabaseColumnName<TEntity>(propertyInfo);
            list.Add($"{dbColumnName}=@{propertyInfo}");
        }

        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        var pkColumnValue = properties.First(x => x.Name == DbPropertyHelper.GetPrimaryKeyPropertyName<TEntity>())
            .GetValue(entity);
        return $"UPDATE {tableName} SET ({string.Join(", ", list)}) WHERE {pkColumnName}={pkColumnValue}";
    }

    internal static string CreateUpdateSpecificColumnSqlQueryStringDapper<TEntity>(ExpandoObject fields, object id,
        string tableName)
        where TEntity : class, IDbEntity, new()
    {
        if (string.IsNullOrWhiteSpace(tableName)) tableName = DbEntityHelper.GetTableName<TEntity>();
        List<string> list = new();
        foreach (var fieldKey in fields.Select(x => x.Key))
        {
            string dbColumnName = DbPropertyHelper.GetDatabaseColumnName<TEntity>(fieldKey);
            list.Add($"{dbColumnName}=@{fieldKey}");
        }

        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        return $"UPDATE {tableName} SET {string.Join(", ", list)} WHERE {pkColumnName}={id}";
    }

    private static string GetSelectPartSqlString<TEntity>()
        where TEntity : class, IDbEntity, new()
    {
        var propertyNameList = typeof(TEntity).GetProperties().Select(x => x.Name).ToList();
        var selectPartOfSqlStrList = new List<string>();
        foreach (var propertyName in propertyNameList)
        {
            var columnName = DbPropertyHelper.GetDatabaseColumnName<TEntity>(propertyName);
            selectPartOfSqlStrList.Add($"[{columnName}] AS [{propertyName}]");
        }

        return string.Join(", ", selectPartOfSqlStrList);
    }

    private static string GetSelectPartSqlString<TEntity, TResult>()
        where TEntity : class, IDbEntity, new() where TResult : class, new()
    {
        var propertyNameList = typeof(TResult).GetProperties().Select(x => x.Name).ToList();
        var selectPartOfSqlStrList = new List<string>();
        foreach (var propertyName in propertyNameList)
        {
            var columnName = DbPropertyHelper.GetDatabaseColumnName<TEntity>(propertyName);
            selectPartOfSqlStrList.Add($"[{columnName}] AS [{propertyName}]");
        }

        return string.Join(", ", selectPartOfSqlStrList);
    }
}