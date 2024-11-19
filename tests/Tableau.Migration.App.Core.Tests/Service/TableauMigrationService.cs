// <copyright file="TableauMigrationService.cs" company="Salesforce, Inc.">
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

namespace TableauMigrationServiceTests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration;
using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.App.Core.Services;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.JsonConverters;
using Tableau.Migration.JsonConverters.SerializableObjects;
using Tableau.Migration.Resources;
using Xunit;

public class TableauMigrationServiceTests
{
    private readonly Mock<IMigrationPlanBuilder> mockPlanBuilder;
    private readonly Mock<IMigrator> mockMigrator;
    private readonly Mock<ILogger<TableauMigrationService>> mockLogger;
    private readonly Mock<IProgressUpdater> mockProgressUpdater;
    private readonly Mock<IProgressMessagePublisher> mockPublisher;
    private readonly Mock<MigrationManifestSerializer> mockManifestSerializer;
    private readonly AppSettings appSettings;
    private readonly TableauMigrationService service;
    private readonly Mock<IFileSystem> mockFileSystem;
    private readonly Mock<ISharedResourcesLocalizer> mockLocalizer;
    private readonly Mock<ILoggerFactory> mockLoggerFactory;
    private readonly Mock<TableauMigrationService> mockTableauMigrationService;

    public TableauMigrationServiceTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTableauMigrationSdk();
        var provider = serviceCollection.BuildServiceProvider();

        var planBuilder = (MigrationPlanBuilder)provider.GetRequiredService<IMigrationPlanBuilder>();
        this.mockPlanBuilder = new Mock<IMigrationPlanBuilder>();
        this.mockPlanBuilder.Setup(pb => pb.Hooks).Returns(planBuilder.Hooks);
        this.mockPlanBuilder.Setup(
            pb => pb.FromSourceTableauServer(
                It.IsAny<Uri>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>())).Returns(
                (Uri url, string site, string tokenName, string token, bool useSim)
                => planBuilder.FromSourceTableauServer(url, site, tokenName, token, true));
        this.mockPlanBuilder.Setup(
            pb => pb.ToDestinationTableauCloud(
                It.IsAny<Uri>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>())).Returns(
                (Uri url, string site, string tokenName, string token, bool useSim)
                => planBuilder.ToDestinationTableauCloud(url, site, tokenName, token, true));
        this.mockMigrator = new Mock<IMigrator>();
        this.mockLogger = new Mock<ILogger<TableauMigrationService>>();
        this.mockProgressUpdater = new Mock<IProgressUpdater>();
        this.mockPublisher = new Mock<IProgressMessagePublisher>();
        this.mockFileSystem = new Mock<IFileSystem>();
        this.mockLocalizer = new Mock<ISharedResourcesLocalizer>();
        this.mockLoggerFactory = new Mock<ILoggerFactory>();
        this.appSettings = new AppSettings { UseSimulator = true };

        this.mockManifestSerializer = new Mock<MigrationManifestSerializer>(provider.GetRequiredService<MigrationManifestSerializer>());
        this.mockTableauMigrationService = new Mock<TableauMigrationService>(
            this.mockPlanBuilder.Object,
            provider.GetRequiredService<IMigrator>(),
            this.mockLogger.Object,
            this.appSettings,
            provider.GetRequiredService<MigrationManifestSerializer>(),
            this.mockProgressUpdater.Object,
            this.mockPublisher.Object)
        { CallBase = true };

        this.service = this.mockTableauMigrationService.Object;
    }

    [Fact]
    public void BuildMigrationPlan_ShouldReturnTrue_WhenValidationSucceeds()
    {
        var serverEndpoints = new EndpointOptions();
        var cloudEndpoints = new EndpointOptions();
        this.mockPlanBuilder.Setup(pb => pb.Validate()).Returns(
            new TestResult(true, ImmutableList<Exception>.Empty));

        var result = this.service.BuildMigrationPlan(serverEndpoints, cloudEndpoints);

        Assert.True(result);
        this.mockPlanBuilder.Verify(pb => pb.Validate(), Times.Once);
    }

    [Fact]
    public void BuildMigrationPlan_ShouldReturnFalse_WhenValidationFails()
    {
        var serverEndpoints = new EndpointOptions();
        var cloudEndpoints = new EndpointOptions();
        this.mockPlanBuilder.Setup(pb => pb.Validate()).Returns(new TestResult(false, ImmutableList<Exception>.Empty));

        var result = this.service.BuildMigrationPlan(serverEndpoints, cloudEndpoints);

        Assert.False(result);
    }

    [Fact]
    public async Task StartMigrationTaskAsync_ShouldReturnFailure_WhenPlanIsNotBuilt()
    {
        var cancelToken = CancellationToken.None;

        var result = await this.service.StartMigrationTaskAsync(cancelToken);

        Assert.Equal(ITableauMigrationService.MigrationStatus.FAILURE, result.status);
    }

    [Fact]
    public async Task SaveManifestAsync_ShouldReturnFalse_WhenManifestIsNull()
    {
        string manifestFilePath = "testPath";

        var result = await this.service.SaveManifestAsync(manifestFilePath);

        Assert.False(result);
    }

    [Fact]
    public async Task LoadManifestAsync_ShouldReturnFalse_WhenManifestFilePathIsInvalid()
    {
        string manifestFilePath = string.Empty;
        var cancelToken = CancellationToken.None;

        var result = await this.service.LoadManifestAsync(manifestFilePath, cancelToken);

        Assert.False(result);
    }

    [Fact]
    public async Task ResumeMigrationTaskAsync_ShouldReturnFailure_WhenManifestIsNotLoaded()
    {
        string manifestFilePath = "testPath";
        var cancelToken = CancellationToken.None;
        this.mockPlanBuilder.Setup(pb => pb.Validate()).Returns(new TestResult(true, ImmutableList<Exception>.Empty));
        var mockMigrationPlan = new Mock<IMigrationPlan>();
        this.mockPlanBuilder.Setup(pb => pb.Build()).Returns(mockMigrationPlan.Object);

        var serverEndpoints = new EndpointOptions();
        var cloudEndpoints = new EndpointOptions();
        this.service.BuildMigrationPlan(serverEndpoints, cloudEndpoints);

        var result = await this.service.ResumeMigrationTaskAsync(manifestFilePath, cancelToken);

        Assert.Equal(ITableauMigrationService.MigrationStatus.FAILURE, result.status);
    }

    [Fact]
    public async Task ResumeMigrationTaskAsync_ShouldReturnFailure_WhenPlanIsNotBuilt()
    {
        string manifestFilePath = "testPath";
        var cancelToken = CancellationToken.None;

        var result = await this.service.ResumeMigrationTaskAsync(manifestFilePath, cancelToken);

        Assert.Equal(ITableauMigrationService.MigrationStatus.FAILURE, result.status);
    }

    [Fact]
    public async Task ResumeMigrationTaskAsync_ShouldExecuteMigration_WhenNoErrors()
    {
        this.mockPlanBuilder.Setup(pb => pb.Validate()).Returns(new TestResult(true, ImmutableList<Exception>.Empty));
        var mockMigrationPlan = new Mock<IMigrationPlan>();
        this.mockPlanBuilder.Setup(pb => pb.Build()).Returns(mockMigrationPlan.Object);

        var planId = Guid.NewGuid();
        var migrationId = Guid.NewGuid();
        var mockManifest = new Mock<MigrationManifest>(
            this.mockLocalizer.Object,
            this.mockLoggerFactory.Object,
            planId,
            migrationId,
            (IMigrationManifest?)null!);
        var manifestTask = Task.FromResult(mockManifest.Object);

        var cancelToken = CancellationToken.None;
        string manifestFilePath = "testPath";

        var result = await this.service.ResumeMigrationTaskAsync(manifestFilePath, cancelToken);

        Assert.Equal(ITableauMigrationService.MigrationStatus.FAILURE, result.status);
    }

    [Fact]
    public void IsMigrationPlanBuilt()
    {
        var isBuilt = this.service.IsMigrationPlanBuilt();
        Assert.False(isBuilt);

        var serverEndpoints = new EndpointOptions();
        var cloudEndpoints = new EndpointOptions();
        this.mockPlanBuilder.Setup(pb => pb.Validate()).Returns(new TestResult(true, ImmutableList<Exception>.Empty));

        var mockMigrationPlan = new Mock<IMigrationPlan>();

        this.mockPlanBuilder.Setup(pb => pb.Build()).Returns(mockMigrationPlan.Object);
        Assert.True(this.service.BuildMigrationPlan(serverEndpoints, cloudEndpoints));

        isBuilt = this.service.IsMigrationPlanBuilt();
        Assert.True(isBuilt);
    }

    internal class TestResult : IResult
    {
        private bool success;
        private IImmutableList<Exception> errors;

        public TestResult(bool success, IImmutableList<Exception> errors)
        {
            this.success = success;
            this.errors = errors;
        }

        public bool Success
        {
            get => this.success;
        }

        public IImmutableList<Exception> Errors
        {
            get => this.errors;
        }
    }
}