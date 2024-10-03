namespace MigrationApp.Core.Tests.Hooks.Mappings;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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