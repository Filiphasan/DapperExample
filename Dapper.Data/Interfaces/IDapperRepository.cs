using System.Dynamic;
using System.Linq.Expressions;
using Dapper.Core.Data.Interfaces;

namespace Dapper.Data.Interfaces;

public interface IDapperRepository<TEntity> where TEntity: class, IDbEntity, new()
{
    Task<IEnumerable<TEntity>> SqlQueryAsync(string sqlQueryString);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(object id);
    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> expression = null);
    Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> expression = null);
    Task<TEntity> GetLastAsync(Expression<Func<TEntity, bool>> expression = null);
    Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression = null);
    Task<TEntity> GetLastOrDefaultAsync(Expression<Func<TEntity, bool>> expression = null);
    Task<TResult> GetByIdAsync<TResult>(object id) where TResult : class, new();
    Task<IEnumerable<TResult>> GetAsync<TResult>(Expression<Func<TEntity, bool>> expression = null) where TResult : class, new();
    Task<TResult> GetFirstAsync<TResult>(Expression<Func<TEntity, bool>> expression = null)  where TResult : class, new();
    Task<TResult> GetLastAsync<TResult>(Expression<Func<TEntity, bool>> expression = null)  where TResult : class, new();
    Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> expression = null)  where TResult : class, new();
    Task<TResult> GetLastOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> expression = null)  where TResult : class, new();
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression);
    Task<int> AddAsync(TEntity entity);
    Task<bool> UpdateAsync(object id, ExpandoObject fields);
    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TEntity entity);
    Task<bool> DeleteAsync(object id);
}