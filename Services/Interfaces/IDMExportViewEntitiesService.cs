using Microsoft.AspNetCore.Mvc;
using SRMDataMigrationIgnite.Models;
using System.Data;
using static SRMDataMigrationIgnite.Services.Repositories.DMExportViewEntitiesService;

namespace SRMDataMigrationIgnite.Services.Interfaces
{
    public interface IDMExportViewEntitiesService
    {        
        Task<DMExportViewEntities> GetViewEntities(Guid viewId, CancellationToken cancellationToken);
        Task<List<ViewEntityCategoryData>> GetRiskCategory(CancellationToken cancellationToken);
        Task<DataTable> GetUserEntities(string viewName, Guid userId);
        Task<List<ViewEntityData>> GetAllUserEntities(Guid userId, CancellationToken cancellationToken);
        Task AddView(DMExportViewEntities dmExport);
        Task DeleteView(DMExportViewEntities dmExport);
    }
}
