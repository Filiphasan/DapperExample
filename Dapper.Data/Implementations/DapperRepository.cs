using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using Dapper.Core.Data.Interfaces;
using Dapper.Data.Interfaces;
using Dapper.Data.Helpers;

namespace Dapper.Data.Implementations;

public class DapperRepository<TEntity> : IDapperRepository<TEntity> where TEntity: class, IDbEntity, new()
{
    private readonly IDbConnection _connection;
    private readonly string _tableName;

    public DapperRepository(IDbConnection connection)
    {
        _connection = connection;
        _tableName = DbEntityHelper.GetTableName<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> SqlQueryAsync(string sqlQueryString)
    {
        return await _connection.QueryAsync<TEntity>(sqlQueryString);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        var parameters = new { TableName = _tableName };
        string sqlQuery = "SELECT * FROM @TableName";
        return await _connection.QueryAsync<TEntity>(sqlQuery, parameters);
    }

    public async Task<TEntity> GetByIdAsync(object id)
    {
        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        var parameters = new { TableName = _tableName, Id = id, PkColumnName = pkColumnName };
        string sqlQuery = "SELECT * FROM @TableName WHERE @PkColumnName=@Id";
        return await _connection.QueryFirstAsync(sqlQuery, parameters);
    }

    public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql<TEntity>(expression);
        var parameters = new { TableName = _tableName, WhereSqlStr = expressionSqlStr };
        string sqlQuery = "SELECT * FROM @TableName WHERE @WhereSqlStr";
        return await _connection.QueryAsync<TEntity>(sqlQuery, parameters);
    }

    public async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql<TEntity>(expression);
        var parameters = new { TableName = _tableName, WhereSqlStr = expressionSqlStr };
        string sqlQuery = "SELECT TOP 1 * FROM @TableName WHERE @WhereSqlStr";
        return await _connection.QueryFirstAsync<TEntity>(sqlQuery, parameters);
    }

    public async Task<TEntity> GetLastAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        var expressionSqlStr = DbExpressionHelper.GetExpressionSql<TEntity>(expression);
        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        var parameters = new { TableName = _tableName, WhereSqlStr = expressionSqlStr, PkColumnName = pkColumnName };
        string sqlQuery = "SELECT TOP 1 * FROM @TableName WHERE @WhereSqlStr ORDER BY @PkColumnName DESC";
        return await _connection.QueryFirstAsync<TEntity>(sqlQuery, parameters);
    }

    public async Task<int> AddAsync(TEntity entity)
    {
        var sqlQuery = DbEntityHelper.CreateInsertSqlQueryStringDapper<TEntity>(_tableName);
        return await _connection.ExecuteAsync(sqlQuery, entity);
    }

    public async Task<bool> UpdateAsync(object id, ExpandoObject fields)
    {
        var sqlQuery = DbEntityHelper.CreateUpdateSpecificColumnSqlQueryStringDapper<TEntity>(fields, id, _tableName);
        var updatedDataCount = await _connection.ExecuteAsync(sqlQuery, fields);
        return updatedDataCount > 0;
    }

    public async Task<bool> UpdateAsync(TEntity entity)
    {
        string sqlQuery = DbEntityHelper.CreateUpdateAllColumnSqlQueryStringDapper(entity, _tableName);
        var updatedDataCount = await _connection.ExecuteAsync(sqlQuery, entity);
        return updatedDataCount > 0;
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        var pkPropertyName = DbPropertyHelper.GetPrimaryKeyPropertyName<TEntity>();
        var pkColumnValue = entity.GetType().GetProperties().First(x => x.Name == pkPropertyName).GetValue(entity);
        var parameters = new { TableName = _tableName, Id = pkColumnValue, PkColumnName = pkColumnName };
        string sqlQuery = "DELETE FROM @TableName WHERE @PkColumnName=@Id";
        var updatedDataCount = await _connection.ExecuteAsync(sqlQuery, parameters);
        return updatedDataCount > 0;
    }

    public async Task<bool> DeleteAsync(object id)
    {
        var pkColumnName = DbPropertyHelper.GetPrimaryKeyColumnName<TEntity>();
        var parameters = new { TableName = _tableName, Id = id, PkColumnName = pkColumnName };
        string sqlQuery = "DELETE FROM @TableName WHERE @PkColumnName=@Id";
        await _connection.QueryFirstAsync(sqlQuery, parameters);
        throw new NotImplementedException();
    }
}