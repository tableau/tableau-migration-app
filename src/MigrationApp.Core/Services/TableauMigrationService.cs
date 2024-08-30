using Microsoft.Extensions.Logging;
using MigrationApp.Core.Entities;
using MigrationApp.Core.Interfaces;
using Tableau.Migration;
namespace MigrationApp.Core.Services
{
    public class TableauMigrationService(IMigrationPlanBuilder planBuilder, IMigrator migrator, ILogger<TableauMigrationService> logger) : ITableauMigrationService
    {
        private readonly IMigrationPlanBuilder _planBuilder = planBuilder;
        private readonly IMigrator _migrator = migrator;
        private readonly ILogger<TableauMigrationService> _logger = logger;
        private IMigrationPlan? _plan;

        public bool BuildMigrationPlan(EndpointOptions serverEndpoints, EndpointOptions cloudEndpoints)
        {
            _planBuilder
                .FromSourceTableauServer(serverEndpoints.Url, serverEndpoints.SiteContentUrl, serverEndpoints.AccessTokenName, serverEndpoints.AccessToken)
                .ToDestinationTableauCloud(cloudEndpoints.Url, cloudEndpoints.SiteContentUrl, cloudEndpoints.AccessTokenName, cloudEndpoints.AccessToken)
                .ForServerToCloud();

            var validationResult = _planBuilder.Validate();

            if (!validationResult.Success)
            {
                _logger.LogError("Migration plan validation failed. {Errors}", validationResult.Errors);
                return false;
            }

            _plan = _planBuilder.Build();

            return true;

        }

        public async Task<bool> StartMigrationTaskAsync(CancellationToken cancel)
        {
            if (_plan == null)
            {
                _logger.LogError("Migration plan is not built.");
                return false;
            }

            var result = await _migrator.ExecuteAsync(_plan, cancel);

            if (result.Status == MigrationCompletionStatus.Completed)
            {
                _logger.LogInformation("Migration succeeded.");
                return true;
            }
            else if (result.Status == MigrationCompletionStatus.Canceled)
            {
                _logger.LogInformation("Migration cancelled.");
                return true;
            }
            else
            {
                _logger.LogError("Migration failed with status: {Status}", result.Status);
                return false;
            }
        }

        public bool IsMigrationPlanBuilt()
        {
            return _plan != null;
        }
    }
}