using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SRMDataMigrationIgnite.Data;
using SRMDataMigrationIgnite.Services.Interfaces;
using System.Data;
using System.Linq.Expressions;

namespace SRMDataMigrationIgnite.Services.Repositories
{
    public class Repository : IRepository
    {        
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public Repository(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public DbContext GetContext() => _context;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<T?> GetByIDAsync<T>(params object[] ids) where T : class
        {
            var t = await _context.Set<T>().FindAsync(ids);
            if (t == null)
                return null;

            return t;
        }

        public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            IQueryable<T> query = _context.Set<T>();

            if (predicate != null)
                query = query.Where(predicate);

            return await query.ToListAsync();
        }

        public IQueryable<T> GetQueryable<T>(bool isTrackable = false) where T : class
        {
            if (isTrackable)
            {
                return _context.Set<T>().AsQueryable();
            }
            return _context.Set<T>().AsQueryable().AsNoTracking();
        }

        public async Task<T> CreateAsync<T>(T entity) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _context.Set<T>().AddAsync(entity);
            await SaveChanges();
            return entity;
        }

        public async Task UpdateAsync<T>(T entity) where T : class
        {
            _context.Update(entity);
            var entry = _context.Entry(entity);
            await SaveChanges();
        }

        public async Task CreateRangeAsync<T>(List<T> entities) where T : class
        {
            _context.Set<T>().AddRange(entities);
            await SaveChanges();
        }

        public async Task<DataTable> LoadDataTableAsync(string sqlQuery)
        {
            var dataTable = new DataTable();
            var connection = _context.Database.GetDbConnection();

            try
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlQuery;
                    command.CommandType = CommandType.Text;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                }
            }
            finally
            {
                await connection.CloseAsync();
            }

            return dataTable;
        }

        public async Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("SQL command cannot be null or empty.", nameof(sql));

            return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }        

        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
