using Microsoft.Extensions.Options;
using MigrationApp.Core.Hooks.Mappings;
using MigrationApp.Core.Interfaces;
using MigrationApp.GUI.Models;
using MigrationApp.GUI.ViewModels;
using Moq;

namespace MigrationApp.GUI.Tests.ViewModels
{
    public class MainWindowViewModelTests
    {
        [Fact]
        public void CloudUserDomain_ShouldUpdateEmailDomainOptions()
        {
            var emailDomainOptionsMock = new Mock<IOptions<EmailDomainMappingOptions>>();
            var optionsValue = new EmailDomainMappingOptions();
            emailDomainOptionsMock.Setup(o => o.Value).Returns(optionsValue);

            var migrationServiceMock = new Mock<ITableauMigrationService>();
            var progressUpdaterMock = new Mock<ProgressUpdater>();
            var viewModel = new MainWindowViewModel(migrationServiceMock.Object, emailDomainOptionsMock.Object, progressUpdaterMock.Object);

            viewModel.CloudUserDomain = "testdomain.com";

            Assert.Equal("testdomain.com", optionsValue.EmailDomain);
        }

        [Fact]
        public void CloudUserDomain_ShouldTriggerValidationErrors_WhenEmpty()
        {
            var emailDomainOptionsMock = new Mock<IOptions<EmailDomainMappingOptions>>();
            var optionsValue = new EmailDomainMappingOptions();
            emailDomainOptionsMock.Setup(o => o.Value).Returns(optionsValue);

            var migrationServiceMock = new Mock<ITableauMigrationService>();
            var progressUpdaterMock = new Mock<ProgressUpdater>();

            var viewModel = new MainWindowViewModel(migrationServiceMock.Object, emailDomainOptionsMock.Object, progressUpdaterMock.Object);

            viewModel.CloudUserDomain = "";

            Assert.NotEmpty(viewModel.GetErrors(nameof(viewModel.CloudUserDomain)));
        }
    }
}
