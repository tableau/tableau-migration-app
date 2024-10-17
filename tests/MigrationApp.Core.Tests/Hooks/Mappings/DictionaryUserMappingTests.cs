// <copyright file="DictionaryUserMappingTests.cs" company="Salesforce, Inc.">
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

namespace MigrationApp.Core.Tests.Hooks.Mappings;

using Microsoft.Extensions.Logging;
using MigrationApp.Core.Hooks.Mappings;
using Moq;
using Tableau.Migration;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Resources;
using Xunit;

public class DictionaryUserMappingTests
{
    [Fact]
    public async Task MapAsync_ShouldMapUserDefinition_WhenDictionaryEntryExists()
    {
        // Setup
        const string sourceUserName = "testUsername";
        const string destinationUserName = "TestUser@destination.com";
        Dictionary<string, string> userMappings =
            new Dictionary<string, string> { { sourceUserName, destinationUserName } };
        var optionsMock = new Mock<IMigrationPlanOptionsProvider<DictionaryUserMappingOptions>>();
        optionsMock.Setup(o => o.Get()).Returns(new DictionaryUserMappingOptions { UserMappings = userMappings });

        var mapping = new DictionaryUserMapping(
            optionsMock.Object,
            Mock.Of<ISharedResourcesLocalizer>(),
            Mock.Of<ILogger<DictionaryUserMapping>>());

        var contentItem = new Mock<IUser>();
        contentItem.Setup(c => c.Name).Returns(sourceUserName);

        var userMappingContext = new ContentMappingContext<IUser>(
            contentItem.Object,
            new ContentLocation("local"));

        // Apply the mapping to the user context
        var result = await mapping.MapAsync(userMappingContext, default);

        // Assert
        Assert.NotNull(result);

        // User should be mapped to new definition from dictionary
        Assert.Equal(destinationUserName, result.MappedLocation.ToString());
    }

    [Fact]
    public async Task MapAsync_ShouldNotMapUserDefinition_WhenDictionaryEntryDoesNotExist()
    {
        // Setup
        const string sourceUserName = "testUsername";
        const string destinationUserName = "TestUser@destination.com";
        Dictionary<string, string> userMappings =
            new Dictionary<string, string> { { "differentUsername", destinationUserName } };
        var optionsMock = new Mock<IMigrationPlanOptionsProvider<DictionaryUserMappingOptions>>();
        optionsMock.Setup(o => o.Get()).Returns(new DictionaryUserMappingOptions { UserMappings = userMappings });

        var mapping = new DictionaryUserMapping(
            optionsMock.Object,
            Mock.Of<ISharedResourcesLocalizer>(),
            Mock.Of<ILogger<DictionaryUserMapping>>());

        var contentItem = new Mock<IUser>();
        contentItem.Setup(c => c.Name).Returns(sourceUserName);

        var userMappingContext = new ContentMappingContext<IUser>(
            contentItem.Object,
            new ContentLocation("local"));

        // Apply the mapping to the user context
        var result = await mapping.MapAsync(userMappingContext, default);

        // Assert
        Assert.NotNull(result);

        // Mapping for user is unaffected
        Assert.Equal("local", result.MappedLocation.ToString());
    }
}