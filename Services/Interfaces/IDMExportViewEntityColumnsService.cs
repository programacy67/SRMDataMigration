using Microsoft.AspNetCore.Mvc;
using SRMDataMigrationIgnite.Models;

namespace SRMDataMigrationIgnite.Services.Interfaces
{
    public interface IDMExportViewEntityColumnsService
    {
        Task<List<DMExportViewEntityColumns>> GetList(Guid viewId);
        Task AddEntityColumns(List<DMExportViewEntityColumns> dmExportColumnsList);
        Task DeleteEntityColumns(Guid viewId, Guid userid);
    }
}
