# Core Module
## Design
The Core module handles the business logic involving performing the migration of Tableau Server to Tableau Cloud.

The Core module has 4 main folders in it:
- **Entities** - Data structures used to represent data flowing through the application.
- **Hooks** - These are hooks that are passed to the Tableau Migration SDK to trigger whenever certain events occur (e.g. when a resource has completed migration).
- **Interfaces** - Defined interfaces implemented in the library.
- **Services** - The main service for injection is located here.

## Entrypoint
The Core module implements the [Microsoft Dependency Injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) pattern.

The core library can by used by configuring the dependency injection with [AddMigrationAppCore](/api/Tableau.Migration.App.Core.ServiceCollectionExtensions.html#Tableau_Migration_App_Core_ServiceCollectionExtensions_AddMigrationAppCore_Microsoft_Extensions_DependencyInjection_IServiceCollection_Microsoft_Extensions_Configuration_IConfiguration_) found in [ServiceCollectionExtensions](/api/Tableau.Migration.App.Core.ServiceCollectionExtensions.html)
.

## Tableau Migration SDK
The connection to Tableau is handled through the [Tableau Migration SDK](https://github.com/tableau/tableau-migration-sdk). The SDK is added as a part of the configuration when the service is injected.

The specifics of the interaction with the SDK entities are defined in [TableauMigrationService](/api/Tableau.Migration.App.Core.Services.TableauMigrationService.html). There, the provided migration details such as the authencation information, and user mappings are set before beginning the migration.
