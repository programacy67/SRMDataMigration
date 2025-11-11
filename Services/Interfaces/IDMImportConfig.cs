using Microsoft.AspNetCore.Mvc;
using SRMDataMigrationIgnite.Models;
using static SRMDataMigrationIgnite.Services.Repositories.DMExportViewEntitiesService;

namespace SRMDataMigrationIgnite.Services.Interfaces
{
    public interface IDMImportConfig
    {
        Task<List<DMTransform>> GetTransformList(CancellationToken cancellationToken);
        Task<DMImportConfig> GetImportConfig(Guid userId, CancellationToken cancellationToken);
        Task AddConfig(DMImportConfig dmImportConfig);
    }
}
