// <copyright file="ConfigLoading.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.MigrationApp.Core.Tests;
public class ConfigLoadTests
{
    [Fact(Skip = "Disabled until TMAPP-53 resolved.")]
    public void ConfigLoadTest()
    {
        // IConfiguration configuration = ServiceCollectionExtensions.BuildConfiguration();
        // using var host = Host.CreateDefaultBuilder()
        //               .ConfigureServices((ctx, services) =>
        //               {
        //                   services.AddTableauMigrationSdk();
        //                   services.AddMigrationAppCore(configuration);
        //               })
        //               .Build();
        // var appSettings = host.Services.GetRequiredService<AppSettings>();
        return;
    }
}