using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace SRMDataMigrationIgnite.Services.Interfaces
{
    public interface IRepository
    {
        /// <summary>
        /// This function is used to get the object by primary keys
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// 
        Task<IDbContextTransaction> BeginTransactionAsync();
        DbContext GetContext();
        Task<int> SaveChanges();
        Task<T?> GetByIDAsync<T>(params object[] ids) where T : class;
        Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class;
        //Task<TResult> GetProjectedAsync<T, TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null) where T : class;
        //Task<List<TResult>> GetDTOListAsync<T, TResult>(Expression<Func<T, TResult>> selector) where T : class;
        //Task<List<TResult>> GetDTOListAsync<T, TResult>(
        //Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null) where T : class;
        Task<T> CreateAsync<T>(T entity) where T : class;
        Task UpdateAsync<T>(T entity) where T : class;
        //Task DeleteAsync<T>(int id) where T : class;
        //Task UpdatePartial<T>(T entity, object updatedProperties) where T : class;
        //Task RemoveAsync<T>(T entity) where T : class;
        //Task<T> FindEntityAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        //Task<bool> DeleteCommonAsync(int id, SRMEnums.SystemAttributes tableName);
        //Task<bool> DeleteCommonAsync(Guid id, SRMEnums.SystemAttributes tableName);

        Task CreateRangeAsync<T>(List<T> entities) where T : class;
        //Task DeleteRangeAsync<T>(List<T> entities) where T : class;
        //Task<List<T>> FindAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

        Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);
        IQueryable<T> GetQueryable<T>(bool isTrackable = false) where T : class;
    }
}
