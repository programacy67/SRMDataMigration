using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SRMDataMigrationIgnite.Data;
using SRMDataMigrationIgnite.Models;
using SRMDataMigrationIgnite.Services.Interfaces;
using SRMDataMigrationIgnite.Utils;
using System.Data;

namespace SRMDataMigrationIgnite.Services.Repositories
{
    public class DMExportViewEntitiesService : IDMExportViewEntitiesService
    {
        string tableName = "DMExportViewEntities";
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public DMExportViewEntitiesService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public DMExportViewEntities GetViewEntities(Guid viewId)
        {
            return _context.dmExportViewEntities.Where(i => i.ID.Equals(viewId)).Where(i => !i.IsArchive).FirstOrDefault();
        }

        public async Task<DataTable> GetRiskCategory()
        {
            var dmColumnList = await _context.dmColumns.OrderBy(dm => dm.ColumnPosition).Where(dm => !dm.IsArchive)
                               .Select(dm => new { dm.CategoryTitle, dm.Title, dm.IsMandatory }).ToListAsync();
            return MiscellaneousService.ToDataTableFromEnum(dmColumnList);
        }

        public async Task<DataTable> GetUserEntities(string viewName)
        {
            if (string.IsNullOrEmpty(viewName) || viewName == "Default")
                viewName = string.Empty;

            var dtUserColumns = new DataTable();
            try
            {
                string sql = $@"SELECT {tableName}.ID, {tableName}.Title AS Views, DMExportViewEntityColumns.Title, DMExportViewEntityColumns.DisplayOrder 
                    FROM {tableName} 
                    INNER JOIN DMExportViewEntityColumns ON DMExportViewEntityColumns.DMExportViewEntityID = {tableName}.ID 
                    AND DMExportViewEntityColumns.IsArchive = 0 
                    WHERE {tableName}.UserID = N'{ApplicationDbContext.userId}' AND {tableName}.IsArchive = 0";
                if (!string.IsNullOrEmpty(viewName))
                    sql = $@"{sql} AND {tableName}.ID = N'{viewName}'";

                if (string.IsNullOrEmpty(viewName))
                    sql = $@"{sql} ORDER BY {tableName}.CreatedOn Desc, DMExportViewEntityColumns.DisplayOrder";
                else
                    sql = $@"{sql} ORDER BY DMExportViewEntityColumns.DisplayOrder ";

                using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        dtUserColumns.Load(reader); // Fill the DataTable with columns + rows
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: " + ex.Message);
            }
            return dtUserColumns;
        }

        public async Task AddView(DMExportViewEntities dmExport)
        {
            _context.dmExportViewEntities.Add(dmExport);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteView(DMExportViewEntities dmExport)
        {
            _context.dmExportViewEntities.Update(dmExport);
            await _context.SaveChangesAsync();
        }
    }
}
