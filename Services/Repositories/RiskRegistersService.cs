using Microsoft.Data.SqlClient;
using SRMDataMigrationIgnite.Data;
using SRMDataMigrationIgnite.Services.Interfaces;
using System.Data;

namespace SRMDataMigrationIgnite.Services.Repositories
{
    public class RiskRegistersService : IRiskRegistersService
    {
        string tableName = "Risk";
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public RiskRegistersService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<DataTable> GetAllRiskRegisters()
        {
            var dt = new DataTable();
            try
            {
                string sql = $@"SELECT Project.UID AS ProjectUID, Project.Title AS ProjectName, {tableName}.UID AS RiskUID, {tableName}.Title, {tableName}.EventDescription, 
                    FORMAT({tableName}.EventDate, 'd', 'en-gb') AS EventDate, {tableName}.Notes, {tableName}.RiskIdentifier, {tableName}.AlternativeRiskIdentifier, 
                    RiskStatus.Title AS Status, 
                    (SELECT RiskCategory.Title + ';' FROM RiskCategory 
                      INNER JOIN RiskCategories on RiskCategories.RiskCategoryID = RiskCategory.ID 
                      WHERE {tableName}.ID = RiskCategories.RiskID FOR XML PATH ('')) AS Category, 

                      (SELECT str(RiskAction.UID) + ';' FROM RiskAction 
                      WHERE {tableName}.ID = RiskAction.RiskID FOR XML PATH('')) AS ActionUID, 
                      (SELECT RiskAction.Title + ';' FROM RiskAction 
                      WHERE {tableName}.ID = RiskAction.RiskID FOR XML PATH('')) AS Action, 

                      (SELECT str(ControlMeasure.UID) + ';' FROM ControlMeasure 
                      WHERE {tableName}.ID = ControlMeasure.RiskID FOR XML PATH('')) AS ControlUID, 
                      (SELECT ControlMeasure.Title + ';' FROM ControlMeasure 
                      WHERE {tableName}.ID = ControlMeasure.RiskID FOR XML PATH('')) AS Control, 

                      (SELECT str(RiskSource.UID) + ';' FROM RiskSource 
                      WHERE {tableName}.ID = RiskSource.RiskID FOR XML PATH('')) AS SourceUID, 
                      (SELECT RiskSource.Title + ';' FROM RiskSource 
                      WHERE {tableName}.ID = RiskSource.RiskID FOR XML PATH('')) AS Source 

                    FROM {tableName} 
                    INNER JOIN Project ON Project.ID = {tableName}.ProjectID 
                    LEFT JOIN RiskStatus ON {tableName}.RiskStatusID = RiskStatus.ID 
                    WHERE Project.ID = N'{ApplicationDbContext.projectId}'";
                
                using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader); // Fill the DataTable with columns + rows
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: " + ex.Message);
            }
            return dt;
        }
    }
}
