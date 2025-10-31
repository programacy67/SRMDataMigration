using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SRMDataMigrationIgnite.Data;
using SRMDataMigrationIgnite.Models;
using SRMDataMigrationIgnite.Services.Interfaces;
using SRMDataMigrationIgnite.Utils;

namespace SRMDataMigrationIgnite.Services.Repositories
{
    public class DMExportViewEntityColumnsService : IDMExportViewEntityColumnsService
    {
        string tableName = "DMExportViewEntityColumns";
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public DMExportViewEntityColumnsService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public List<DMExportViewEntityColumns> GetList(Guid viewId)
        {
            return _context.dmExportViewEntityColumns.Where(i => i.DMExportViewEntityID.Equals(viewId)).ToList();
        }

        public async Task AddEntityColumns(List<DMExportViewEntityColumns> dmExportColumnsList)
        {
            try
            {
                using var reader = new ObjectDataReader<DMExportViewEntityColumns>(dmExportColumnsList);
                var connectionString = _config.GetConnectionString("DefaultConnection");
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                using var bulkCopy = new SqlBulkCopy(connection)
                {
                    DestinationTableName = "DMExportViewEntityColumns"
                };
                await bulkCopy.WriteToServerAsync(reader);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: " + ex.Message);
            }
        }

        public async Task DeleteEntityColumns(Guid viewId)
        {
            try
            {
                Guid userid = ApplicationDbContext.userId;
                DateTime dtNow = DateTime.Now;
                string sql = $@"UPDATE {tableName} SET ModifiedBy = N'{userid}', ModifiedOn = N'{dtNow}', IsArchive = 1 
                    WHERE DMExportViewEntityID = N'{viewId}' AND IsArchive = 0";

                using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    await conn.OpenAsync();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: " + ex.Message);
            }
        }
    }
}
