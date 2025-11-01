using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SRMDataMigrationIgnite.Data;
using SRMDataMigrationIgnite.Models;
using SRMDataMigrationIgnite.Services.Interfaces;
using SRMDataMigrationIgnite.Utils;

namespace SRMDataMigrationIgnite.Services.Repositories
{
    public class DMExportViewEntityColumnsService : IDMExportViewEntityColumnsService
    {
        string tableName = "DMExportViewEntityColumns";
        private readonly IRepository _repository;
        private readonly ILogger<DMExportViewEntitiesService> _logger;

        public DMExportViewEntityColumnsService(IRepository repository, ILogger<DMExportViewEntitiesService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<DMExportViewEntityColumns>> GetList(Guid viewId)
        {
            return await _repository.GetListAsync<DMExportViewEntityColumns>(i => i.DMExportViewEntityID.Equals(viewId));
        }

        public async Task AddEntityColumns(List<DMExportViewEntityColumns> dmExportColumnsList)
        {
            try
            {
                await _repository.CreateRangeAsync(dmExportColumnsList);
                await _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: " + ex.Message);
            }
        }

        public async Task DeleteEntityColumns(Guid viewId, Guid userid)
        {
            try
            {
                DateTime dtNow = DateTime.UtcNow;
                string sql = $@"UPDATE {tableName} SET ModifiedBy = N'{userid.ToString()}', ModifiedOn = CONVERT (DATETIME, '{dtNow}', 105), IsArchive = 1 
                    WHERE DMExportViewEntityID = N'{viewId.ToString()}' AND IsArchive = 0";

                await _repository.ExecuteSqlCommandAsync(sql);
                await _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: " + ex.Message);
            }
        }
    }
}
