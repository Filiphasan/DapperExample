using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using Dapper.Core.Data.Interfaces;
using Dapper.Data.Interfaces;
using Dapper.Data.Helpers;

namespace Dapper.Data.Implementations;

public class DapperRepository<TEntity> : IDapperRepository<TEntity> where TEntity: class, IDbEntity, new()
{
    protected readonly IDbConnection Connection;
    private readonly string _tableName;

    public DapperRepository(IDbConnection connection)
    {
        Connection = connection;
        _tableName = DbEntityHelper.GetTableName<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> SqlQueryAsync(string sqlQueryString)
    {
        return await Connection.QueryAsync<TEntity>(sqlQueryString);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        var parameters = new { TableName = _tableName };
        string sqlQuery = "SELECT * FROM @TableName";
        return await Connection.QueryAsync<TEntity>(sqlQuery, parameters);
    }

    public async Task<TEntity> GetByIdAsync(object id)
    {
        string sqlQuery = DbSqlBuilderHelper.GetByIdSqlString<TEntity>(_tableName, id);
        return await Connection.QueryFirstAsync<TEntity>(sqlQuery);
    }

    public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> expression = null)
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql(expression);
        string sqlQuery = DbSqlBuilderHelper.GetSqlString<TEntity>(_tableName, expressionSqlStr);
        return await Connection.QueryAsync<TEntity>(sqlQuery);
    }

    public async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> expression = null)
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql(expression);
        string sqlQuery = DbSqlBuilderHelper.GetFirstSqlString<TEntity>(_tableName, expressionSqlStr);
        return await Connection.QueryFirstAsync<TEntity>(sqlQuery);
    }

    public async Task<TEntity> GetLastAsync(Expression<Func<TEntity, bool>> expression = null)
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql(expression);
        string sqlQuery = DbSqlBuilderHelper.GetLastSqlString<TEntity>(_tableName, expressionSqlStr);
        return await Connection.QueryFirstAsync<TEntity>(sqlQuery);
    }
    
    public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression = null)
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql(expression);
        string sqlQuery = DbSqlBuilderHelper.GetFirstSqlString<TEntity>(_tableName, expressionSqlStr);
        return await Connection.QueryFirstOrDefaultAsync<TEntity>(sqlQuery);
    }

    public async Task<TEntity> GetLastOrDefaultAsync(Expression<Func<TEntity, bool>> expression = null)
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql(expression);
        string sqlQuery = DbSqlBuilderHelper.GetLastSqlString<TEntity>(_tableName, expressionSqlStr);
        return await Connection.QueryFirstOrDefaultAsync<TEntity>(sqlQuery);
    }

    public async Task<TResult> GetByIdAsync<TResult>(object id) where TResult : class, new()
    {
        string sqlQuery = DbSqlBuilderHelper.GetByIdSqlString<TEntity, TResult>(_tableName, id);
        return await Connection.QueryFirstAsync<TResult>(sqlQuery);
    }

    public async Task<IEnumerable<TResult>> GetAsync<TResult>(Expression<Func<TEntity, bool>> expression = null) where TResult : class, new()
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql(expression);
        string sqlQuery = DbSqlBuilderHelper.GetSqlString<TEntity, TResult>(_tableName, expressionSqlStr);
        return await Connection.QueryAsync<TResult>(sqlQuery);
    }

    public async Task<TResult> GetFirstAsync<TResult>(Expression<Func<TEntity, bool>> expression = null)  where TResult : class, new()
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql(expression);
        string sqlQuery = DbSqlBuilderHelper.GetFirstSqlString<TEntity, TResult>(_tableName, expressionSqlStr);
        return await Connection.QueryFirstAsync<TResult>(sqlQuery);
    }

    public async Task<TResult> GetLastAsync<TResult>(Expression<Func<TEntity, bool>> expression = null)  where TResult : class, new()
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql(expression);
        string sqlQuery = DbSqlBuilderHelper.GetLastSqlString<TEntity, TResult>(_tableName, expressionSqlStr);
        return await Connection.QueryFirstAsync<TResult>(sqlQuery);
    }

    public async Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> expression = null)  where TResult : class, new()
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql(expression);
        string sqlQuery = DbSqlBuilderHelper.GetFirstSqlString<TEntity, TResult>(_tableName, expressionSqlStr);
        return await Connection.QueryFirstOrDefaultAsync<TResult>(sqlQuery);
    }

    public async Task<TResult> GetLastOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> expression = null)  where TResult : class, new()
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql(expression);
        string sqlQuery = DbSqlBuilderHelper.GetLastSqlString<TEntity, TResult>(_tableName, expressionSqlStr);
        return await Connection.QueryFirstOrDefaultAsync<TResult>(sqlQuery);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression)
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql(expression);
        string sqlQuery = DbSqlBuilderHelper.GetAnySqlString(_tableName, expressionSqlStr);
        return await Connection.QueryFirstAsync<bool>(sqlQuery);
    }

    public async Task<int> AddAsync(TEntity entity)
    {
        var sqlQuery = DbSqlBuilderHelper.CreateInsertSqlQueryStringDapper<TEntity>(_tableName);
        return await Connection.ExecuteAsync(sqlQuery, entity);
    }

    public async Task<bool> UpdateAsync(object id, ExpandoObject fields)
    {
        var sqlQuery = DbSqlBuilderHelper.CreateUpdateSpecificColumnSqlQueryStringDapper<TEntity>(fields, id, _tableName);
        var updatedDataCount = await Connection.ExecuteAsync(sqlQuery, fields);
        return updatedDataCount > 0;
    }

    public async Task<bool> UpdateAsync(TEntity entity)
    {
        string sqlQuery = DbSqlBuilderHelper.CreateUpdateAllColumnSqlQueryStringDapper(entity, _tableName);
        var updatedDataCount = await Connection.ExecuteAsync(sqlQuery, entity);
        return updatedDataCount > 0;
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        var pkColumnValue = DbPropertyHelper.GetPrimaryKeyColumnValue(entity);
        string sqlQuery = DbSqlBuilderHelper.GetDeleteSqlString<TEntity>(_tableName, pkColumnValue);
        var updatedDataCount = await Connection.ExecuteAsync(sqlQuery);
        return updatedDataCount > 0;
    }

    public async Task<bool> DeleteAsync(object id)
    {
        string sqlQuery = DbSqlBuilderHelper.GetDeleteSqlString<TEntity>(_tableName, id);
        var updatedDataCount = await Connection.ExecuteAsync(sqlQuery);
        return updatedDataCount > 0;
    }
}