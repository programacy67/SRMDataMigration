using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SRMDataMigrationIgnite.Data;
using SRMDataMigrationIgnite.Models;
using SRMDataMigrationIgnite.Services.Interfaces;
using SRMDataMigrationIgnite.Utils;
using System.Data;
using System.Threading;

namespace SRMDataMigrationIgnite.Services.Repositories
{
    public class DMExportViewEntitiesService : IDMExportViewEntitiesService
    {
        string tableName = "DMExportViewEntities";
        string tableNameEntityColumns = "DMExportViewEntityColumns";
        private readonly IRepository _repository;
        private readonly ILogger<DMExportViewEntitiesService> _logger;

        public DMExportViewEntitiesService(IRepository repository, ILogger<DMExportViewEntitiesService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<DMExportViewEntities> GetViewEntities(Guid viewId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _repository.GetQueryable<DMExportViewEntities>()
                    .FirstOrDefaultAsync(i => i.ID.Equals(viewId) && !i.IsArchive, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: " + ex.Message);
            }
        }

        public async Task<List<ViewEntityCategoryData>> GetRiskCategory(CancellationToken cancellationToken = default)
        {
            return await _repository.GetQueryable<DMColumns>()
                    .Where(dm => !dm.IsArchive).OrderBy(dm => dm.ColumnPosition)
                    .Select(dm => new ViewEntityCategoryData
                    {
                        CategoryTitle = dm.CategoryTitle,
                        Title = dm.Title,
                        IsMandatory = dm.IsMandatory
                    }).ToListAsync(cancellationToken);
        }

        public async Task<DataTable> GetUserEntities(string viewName, Guid userId)
        {
            if (string.IsNullOrEmpty(viewName) || viewName == "Default")
                viewName = string.Empty;

            var dtUserColumns = new DataTable();
            try
            {
                string sql = $@"SELECT {tableName}.ID, {tableName}.Title AS Views, {tableNameEntityColumns}.Title, {tableNameEntityColumns}.DisplayOrder 
                    FROM {tableName} 
                    INNER JOIN {tableNameEntityColumns} ON {tableNameEntityColumns}.DMExportViewEntityID = {tableName}.ID 
                    AND {tableNameEntityColumns}.IsArchive = 0 
                    WHERE {tableName}.UserID = N'{userId.ToString()}' AND {tableName}.IsArchive = 0";

                if (!string.IsNullOrEmpty(viewName))
                    sql = $@"{sql} AND {tableName}.ID = N'{viewName}'";

                if (string.IsNullOrEmpty(viewName))
                    sql = $@"{sql} ORDER BY {tableName}.CreatedOn Desc, {tableNameEntityColumns}.DisplayOrder";
                else
                    sql = $@"{sql} ORDER BY {tableNameEntityColumns}.DisplayOrder ";

                dtUserColumns = await _repository.LoadDataTableAsync(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: " + ex.Message);
            }
            return dtUserColumns;
        }

        public async Task<List<ViewEntityData>> GetAllUserEntities(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _repository.GetQueryable<DMExportViewEntities>()
                    .Where(dm => !dm.IsArchive)
                    .Select(dm => new ViewEntityData
                    { 
                        Title = dm.Title, 
                        ID = dm.ID 
                    }).ToListAsync(cancellationToken);
        }

        public async Task AddView(DMExportViewEntities newData)
        {
            await _repository.CreateAsync(newData);
            await _repository.SaveChanges();
        }

        public async Task DeleteView(DMExportViewEntities dmExport)
        {
            await _repository.UpdateAsync(dmExport);
            await _repository.SaveChanges();
        }


        #region Helper Classes
        public class ViewEntityData
        {
            public Guid ID { get; set; }
            public string Title { get; set; }
        }

        public class ViewEntityCategoryData
        {
            public string CategoryTitle { get; set; }
            public string Title { get; set; }
            public bool IsMandatory { get; set; }
        }
        #endregion
    }
}
