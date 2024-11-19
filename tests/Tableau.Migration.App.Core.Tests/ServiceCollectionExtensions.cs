// <copyright file="ServiceCollectionExtensions.cs" company="Salesforce, Inc.">
// Copyright (c) 2024, Salesforce, Inc. All rights reserved.
// SPDX-License-Identifier: Apache-2
//
// Licensed under the Apache License, Version 2.0 (the "License")
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at:
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace ServiceCollectionExtensionsTests;

using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.App.Core;
using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.Core.Hooks.Mappings;
using Tableau.Migration.App.Core.Hooks.Progression;
using Tableau.Migration.App.Core.Interfaces;
using Xunit;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMigrationAppCore_Services()
    {
        var serviceCollection = new ServiceCollection();
        var configuration = ServiceCollectionExtensions.BuildConfiguration();
        serviceCollection.AddMigrationAppCore(configuration);
        var provider = serviceCollection.BuildServiceProvider();

        // Verify that the set of dependencies are injected
        Assert.NotNull(provider.GetRequiredService<AppSettings>());
        Assert.NotNull(provider.GetRequiredService<ITableauMigrationService>());
        Assert.NotNull(provider.GetRequiredService<EmailDomainMapping>());
        Assert.NotNull(provider.GetRequiredService<MigrationActionProgressHook>());

        // DicitonaryUserMapping needs to be verified through the collection as the
        // service can't be retrieved before Tableau Migration SDK entities are set up.
        Assert.Contains(
            serviceCollection,
            service => service.ServiceType == typeof(DictionaryUserMapping));
    }
}