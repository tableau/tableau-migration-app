// <copyright file="EmailDomainMappingTests.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core.Tests.Hooks.Mappings
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MigrationApp.Core.Hooks.Mappings;
    using Moq;
    using Tableau.Migration;
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