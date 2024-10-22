// <copyright file="EmailDomainMappingTests.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.Core.Tests.Hooks.Mappings
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using Tableau.Migration;
    using Tableau.Migration.App.Core.Hooks.Mappings;
    using Tableau.Migration.Content;
    using Tableau.Migration.Engine.Hooks.Mappings;
    using Tableau.Migration.Resources;
    using Xunit;

    public class EmailDomainMappingTests
    {
        [Fact]
        public async Task MapAsync_ShouldAppendEmailDomain_WhenNoEmailExists()
        {
            var optionsMock = new Mock<IOptions<EmailDomainMappingOptions>>();
            optionsMock.Setup(o => o.Value).Returns(new EmailDomainMappingOptions { EmailDomain = "test.com" });

            var loggerMock = new Mock<ILogger<EmailDomainMapping>>();
            var localizerMock = new Mock<ISharedResourcesLocalizer>();

            var mapping = new EmailDomainMapping(optionsMock.Object, localizerMock.Object, loggerMock.Object);

            var contentItem = new Mock<IUser>();
            contentItem.Setup(c => c.Email).Returns(string.Empty);  // No email exists
            contentItem.Setup(c => c.Name).Returns("johndoe");

            var mappedLocation = new ContentLocation("dummy/project/path");

            var userMappingContext = new ContentMappingContext<IUser>(contentItem.Object, mappedLocation);

            var result = await mapping.MapAsync(userMappingContext, default);

            Assert.NotNull(result);
            Assert.Contains("johndoe@test.com", result.MappedLocation.ToString());
        }

        [Fact]
        public async Task MapAsync_ShouldUseExistingEmail_WhenEmailAlreadyExists()
        {
            var optionsMock = new Mock<IOptions<EmailDomainMappingOptions>>();
            optionsMock.Setup(o => o.Value).Returns(new EmailDomainMappingOptions { EmailDomain = "test.com" });

            var loggerMock = new Mock<ILogger<EmailDomainMapping>>();
            var localizerMock = new Mock<ISharedResourcesLocalizer>();

            var mapping = new EmailDomainMapping(optionsMock.Object, localizerMock.Object, loggerMock.Object);

            var contentItem = new Mock<IUser>();
            contentItem.Setup(c => c.Email).Returns("existingemail@existingdomain.com");  // Email already exists
            contentItem.Setup(c => c.Name).Returns("johndoe");

            var mappedLocation = new ContentLocation("dummy/project/path");

            var userMappingContext = new ContentMappingContext<IUser>(contentItem.Object, mappedLocation);

            var result = await mapping.MapAsync(userMappingContext, default);

            Assert.NotNull(result);
            Assert.Contains("existingemail@existingdomain.com", result.MappedLocation.ToString());
        }

        [Fact]
        public async Task MapAsync_ShouldUseNameAsEmail_WhenNameIsAlreadyEmailFormat()
        {
            var optionsMock = new Mock<IOptions<EmailDomainMappingOptions>>();
            optionsMock.Setup(o => o.Value).Returns(new EmailDomainMappingOptions { EmailDomain = "test.com" });

            var loggerMock = new Mock<ILogger<EmailDomainMapping>>();
            var localizerMock = new Mock<ISharedResourcesLocalizer>();

            var mapping = new EmailDomainMapping(optionsMock.Object, localizerMock.Object, loggerMock.Object);

            var contentItem = new Mock<IUser>();
            contentItem.Setup(c => c.Email).Returns(string.Empty);
            contentItem.Setup(c => c.Name).Returns("johndoe@nottest.com");

            var mappedLocation = new ContentLocation("dummy/project/path");

            var userMappingContext = new ContentMappingContext<IUser>(contentItem.Object, mappedLocation);

            var result = await mapping.MapAsync(userMappingContext, default);

            Assert.NotNull(result);
            Assert.Contains("johndoe@nottest.com", result.MappedLocation.ToString());
        }
    }
}