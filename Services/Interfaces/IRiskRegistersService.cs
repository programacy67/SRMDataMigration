using System.Data;

namespace SRMDataMigrationIgnite.Services.Interfaces
{
    public interface IRiskRegistersService
    {
        Task<DataTable> GetAllRiskRegisters();
    }
}
