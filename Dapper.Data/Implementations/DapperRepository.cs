using System.Data;
using System.Linq.Expressions;
using Dapper.Core.Data.Interfaces;
using Dapper.Data.Interfaces;
using Dapper;

namespace Dapper.Data.Implementations;

public class DapperRepository<TEntity> : IDapperRepository<TEntity> where TEntity: class, IDbEntity, new()
{
    private readonly IDbConnection _connection;

    public DapperRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<TEntity>> SqlQueryAsync(string sqlQueryString)
    {
        return await _connection.QueryAsync<TEntity>(sqlQueryString);
    }

    public Task<IEnumerable<TEntity>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> GetByIdAsync(object id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> GetLastAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        throw new NotImplementedException();
    }

    public Task<int> AddAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(object id, dynamic fields)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(object id)
    {
        throw new NotImplementedException();
    }
}