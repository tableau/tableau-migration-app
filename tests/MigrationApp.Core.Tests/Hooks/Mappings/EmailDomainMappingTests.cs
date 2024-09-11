using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MigrationApp.Core.Hooks.Mappings;
using Tableau.Migration.Content;
using Tableau.Migration.Resources;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration;

namespace MigrationApp.Core.Tests.Hooks.Mappings
{
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
            contentItem.Setup(c => c.Email).Returns("");  // No email exists
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
    }
}
