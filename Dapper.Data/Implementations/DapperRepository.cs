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
        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        var parameters = new { TableName = _tableName, Id = id, PkColumnName = pkColumnName };
        string sqlQuery = "SELECT * FROM @TableName WHERE @PkColumnName=@Id";
        return await Connection.QueryFirstAsync<TEntity>(sqlQuery, parameters);
    }

    public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql<TEntity>(expression);
        var parameters = new { TableName = _tableName, WhereSqlStr = expressionSqlStr };
        string sqlQuery = "SELECT * FROM @TableName WHERE @WhereSqlStr";
        return await Connection.QueryAsync<TEntity>(sqlQuery, parameters);
    }

    public async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql<TEntity>(expression);
        var parameters = new { TableName = _tableName, WhereSqlStr = expressionSqlStr };
        string sqlQuery = "SELECT TOP 1 * FROM @TableName WHERE @WhereSqlStr";
        return await Connection.QueryFirstAsync<TEntity>(sqlQuery, parameters);
    }

    public async Task<TEntity> GetLastAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql<TEntity>(expression);
        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        var parameters = new { TableName = _tableName, WhereSqlStr = expressionSqlStr, PkColumnName = pkColumnName };
        string sqlQuery = "SELECT TOP 1 * FROM @TableName WHERE @WhereSqlStr ORDER BY @PkColumnName DESC";
        return await Connection.QueryFirstAsync<TEntity>(sqlQuery, parameters);
    }
    
    public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql<TEntity>(expression);
        var parameters = new { TableName = _tableName, WhereSqlStr = expressionSqlStr };
        string sqlQuery = "SELECT TOP 1 * FROM @TableName WHERE @WhereSqlStr";
        return await Connection.QueryFirstOrDefaultAsync<TEntity>(sqlQuery, parameters);
    }

    public async Task<TEntity?> GetLastOrDefaultAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql<TEntity>(expression);
        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        var parameters = new { TableName = _tableName, WhereSqlStr = expressionSqlStr, PkColumnName = pkColumnName };
        string sqlQuery = "SELECT TOP 1 * FROM @TableName WHERE @WhereSqlStr ORDER BY @PkColumnName DESC";
        return await Connection.QueryFirstOrDefaultAsync<TEntity>(sqlQuery, parameters);
    }

    public async Task<int> AddAsync(TEntity entity)
    {
        var sqlQuery = DbEntityHelper.CreateInsertSqlQueryStringDapper<TEntity>(_tableName);
        return await Connection.ExecuteAsync(sqlQuery, entity);
    }

    public async Task<bool> UpdateAsync(object id, ExpandoObject fields)
    {
        var sqlQuery = DbEntityHelper.CreateUpdateSpecificColumnSqlQueryStringDapper<TEntity>(fields, id, _tableName);
        var updatedDataCount = await Connection.ExecuteAsync(sqlQuery, fields);
        return updatedDataCount > 0;
    }

    public async Task<bool> UpdateAsync(TEntity entity)
    {
        string sqlQuery = DbEntityHelper.CreateUpdateAllColumnSqlQueryStringDapper(entity, _tableName);
        var updatedDataCount = await Connection.ExecuteAsync(sqlQuery, entity);
        return updatedDataCount > 0;
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        var pkPropertyName = DbPropertyHelper.GetPrimaryKeyPropertyName<TEntity>();
        var pkColumnValue = entity.GetType().GetProperties().First(x => x.Name == pkPropertyName).GetValue(entity);
        var parameters = new { TableName = _tableName, Id = pkColumnValue, PkColumnName = pkColumnName };
        string sqlQuery = "DELETE FROM @TableName WHERE @PkColumnName=@Id";
        var updatedDataCount = await Connection.ExecuteAsync(sqlQuery, parameters);
        return updatedDataCount > 0;
    }

    public async Task<bool> DeleteAsync(object id)
    {
        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        var parameters = new { TableName = _tableName, Id = id, PkColumnName = pkColumnName };
        string sqlQuery = "DELETE FROM @TableName WHERE @PkColumnName=@Id";
        await Connection.ExecuteAsync(sqlQuery, parameters);
        var updatedDataCount = await Connection.ExecuteAsync(sqlQuery, parameters);
        return updatedDataCount > 0;
    }
}