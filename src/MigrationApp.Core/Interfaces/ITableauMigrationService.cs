using MigrationApp.Core.Entities;

namespace MigrationApp.Core.Interfaces
{
    public interface ITableauMigrationService
    {
        bool BuildMigrationPlan(EndpointOptions serverCreds, EndpointOptions cloudCreds);

        Task<bool> StartMigrationTaskAsync(CancellationToken cancel);

        bool IsMigrationPlanBuilt();
    }
}
