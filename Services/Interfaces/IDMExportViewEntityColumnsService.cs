using Microsoft.AspNetCore.Mvc;
using SRMDataMigrationIgnite.Models;

namespace SRMDataMigrationIgnite.Services.Interfaces
{
    public interface IDMExportViewEntityColumnsService
    {
        List<DMExportViewEntityColumns> GetList(Guid viewId);
        Task AddEntityColumns(List<DMExportViewEntityColumns> dmExportColumnsList);
        Task DeleteEntityColumns(Guid viewId);
    }
}
