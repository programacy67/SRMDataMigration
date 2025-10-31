using Microsoft.AspNetCore.Mvc;
using SRMDataMigrationIgnite.Models;
using System.Data;

namespace SRMDataMigrationIgnite.Services.Interfaces
{
    public interface IDMExportViewEntitiesService
    {        
        DMExportViewEntities GetViewEntities(Guid viewId);
        Task<DataTable> GetRiskCategory();

        Task<DataTable> GetUserEntities(string viewName);

        Task AddView(DMExportViewEntities dmExport);

        Task DeleteView(DMExportViewEntities dmExport);
    }
}
