using System.Dynamic;
using System.Linq.Expressions;
using Dapper.Core.Data.Interfaces;

namespace Dapper.Data.Interfaces;

public interface IDapperRepository<TEntity> where TEntity: class, IDbEntity, new()
{
    Task<IEnumerable<TEntity>> SqlQueryAsync(string sqlQueryString);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(object id);
    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? expression = null);
    Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>>? expression = null);
    Task<TEntity> GetLastAsync(Expression<Func<TEntity, bool>>? expression = null);
    Task<int> AddAsync(TEntity entity);
    Task<bool> UpdateAsync(object id, ExpandoObject fields);
    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TEntity entity);
    Task<bool> DeleteAsync(object id);
}