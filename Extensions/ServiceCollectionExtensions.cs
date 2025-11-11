using SRMDataMigrationIgnite.Models;
using SRMDataMigrationIgnite.Services.Interfaces;
using SRMDataMigrationIgnite.Services.Repositories;

namespace SRMDataMigrationIgnite.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Business layer
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IRiskRegistersService, RiskRegistersService>();
            services.AddScoped<IDMExportViewEntitiesService, DMExportViewEntitiesService>();
            services.AddScoped<IDMExportViewEntityColumnsService, DMExportViewEntityColumnsService>();
            services.AddScoped<IDMImportConfig, DMImportConfigService>();
            return services;
        }
    }
}
