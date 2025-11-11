using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using SRMDataMigrationIgnite.Models;
using SRMDataMigrationIgnite.Services.Interfaces;
using static SRMDataMigrationIgnite.Services.Repositories.DMExportViewEntitiesService;

namespace SRMDataMigrationIgnite.Services.Repositories
{
    public class DMImportConfigService : IDMImportConfig
    {
        private readonly IRepository _repository;
        private readonly ILogger<DMImportConfigService> _logger;

        public DMImportConfigService(IRepository repository, ILogger<DMImportConfigService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<DMTransform>> GetTransformList(CancellationToken cancellationToken)
        {
            return await _repository.GetQueryable<DMTransform>()
                    .Where(dm => !dm.IsArchive).ToListAsync(cancellationToken);
        }

        public async Task AddConfig(Models.DMImportConfig dmImportConfig)
        {
            throw new NotImplementedException();
        }

        public async Task<DMImportConfig> GetImportConfig(Guid userId, CancellationToken cancellationToken)
        {
            return await _repository.GetQueryable<DMImportConfig>()
                    .Where(dm => !dm.IsArchive && dm.UserID == userId).OrderByDescending(dm => dm.CreatedOn)
                    .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
