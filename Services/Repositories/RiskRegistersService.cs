using Microsoft.Data.SqlClient;
using SRMDataMigrationIgnite.Data;
using SRMDataMigrationIgnite.Services.Interfaces;
using System.Data;

namespace SRMDataMigrationIgnite.Services.Repositories
{
    public class RiskRegistersService : IRiskRegistersService
    {
        string tableName = "Risk";
        private readonly IRepository _repository;
        private readonly ILogger<DMExportViewEntitiesService> _logger;

        public RiskRegistersService(IRepository repository, ILogger<DMExportViewEntitiesService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<DataTable> GetAllRiskRegisters(Guid projectId)
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
                    WHERE Project.ID = N'{projectId.ToString()}'";

                dt = await _repository.LoadDataTableAsync(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: " + ex.Message);
            }
            return dt;
        }
    }
}
