using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SRMDataMigrationIgnite.Services.Interfaces;
using System.Linq.Expressions;

namespace SRMDataMigrationIgnite.Services.Repositories
{
    public class Repository : IRepository
    {
        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<T> CreateAsync<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }

        public Task CreateRangeAsync<T>(List<T> entities) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public Task<T?> GetByIDAsync<T>(params object[] ids) where T : class
        {
            throw new NotImplementedException();
        }

        public DbContext GetContext()
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetQueryable<T>(bool isTrackable = false) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChanges()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
