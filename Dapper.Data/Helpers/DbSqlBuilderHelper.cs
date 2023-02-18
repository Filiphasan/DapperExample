using Dapper.Core.Data.Interfaces;

namespace Dapper.Data.Helpers;

internal static class DbSqlBuilderHelper
{
    internal static string GetFirstSqlString<TEntity>(string tableName, string whereConditionStr)
        where TEntity : class, IDbEntity, new()
    {
        string resultStr;
        var propertyNameList = typeof(TEntity).GetProperties().Select(x => x.Name);
        var selectPartOfSqlStrList = new List<string>();
        foreach (var propertyName in propertyNameList)
        {
            var columnName = DbPropertyHelper.GetDatabaseColumnName<TEntity>(propertyName);
            selectPartOfSqlStrList.Add($"[{columnName}] AS [{propertyName}]");
        }

        resultStr = string.Format("SELECT TOP 1 {0} FROM {1}", string.Join(", ", selectPartOfSqlStrList), tableName);
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

    internal static string GetDeleteSqlString<TEntity>(string tableName, object id)
        where TEntity : class, IDbEntity, new()
    {
        string resultStr;
        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        resultStr = string.Format("DELETE FROM [{0}] WHERE {1}={2}", tableName, pkColumnName, id);
        return resultStr;
    }
}