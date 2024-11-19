// <copyright file="BatchMigrationCompletedProgressHook.cs" company="Salesforce, Inc.">
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

namespace BatchMigrationCompletedProgressHookTests;

using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;
using System.Threading;
using Tableau.Migration;
using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.Core.Hooks.Progression;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Migrators.Batch;
using Xunit;
using Xunit.Abstractions;

public class BatchMigrationCompletedProgressHookTests
{
    private readonly ITestOutputHelper output;

    public BatchMigrationCompletedProgressHookTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void BatchMigrationProgressHook_Execute_Messages()
    {
        var publisher = new TestPublisher();
        var logger = new Mock<ILogger<BatchMigrationCompletedProgressHook<IUser>>>();
        var hook = new BatchMigrationCompletedProgressHook<IUser>(logger.Object, publisher);

        var sourceLocation = new ContentLocation(["sourceSegmentOne", "sourceSegmentTwo"]);
        var destLocation = new ContentLocation(["destSegmentOne", "destSegmentTwo"]);

        // Mock the results
        var item = new Mock<IContentItemMigrationResult<IUser>>();
        item.Setup(item => item.ManifestEntry.Source.Location).Returns(sourceLocation);
        item.Setup(item => item.ManifestEntry.MappedLocation).Returns(destLocation);
        item.Setup(item => item.ManifestEntry.Status).Returns(MigrationManifestEntryStatus.Migrated);
        item.Setup(item => item.ManifestEntry.Errors).Returns([]);

        var itemResults = ImmutableList.Create(item.Object);

        var result = new Mock<IContentBatchMigrationResult<IUser>>();
        result.Setup(ctx => ctx.ItemResults).Returns(itemResults);

        hook.ExecuteAsync(result.Object, CancellationToken.None);
        Assert.Single(publisher.Actions);
        Assert.Single(publisher.Messages);
        Assert.Equal(["User"], publisher.Actions);
        Assert.Contains(" \U0001F7E2 [sourceSegmentTwo] to [destSegmentTwo] → Migrated", publisher.Messages[0]);
    }

    [Fact]
    public void BatchMigrationPtogressHook_Execute_Errors_UnknownFormat()
    {
        var publisher = new TestPublisher();
        var logger = new Mock<ILogger<BatchMigrationCompletedProgressHook<IUser>>>();
        var hook = new BatchMigrationCompletedProgressHook<IUser>(logger.Object, publisher);

        var sourceLocation = new ContentLocation(["sourceSegmentOne", "sourceSegmentTwo"]);
        var destLocation = new ContentLocation(["destSegmentOne", "destSegmentTwo"]);

        // Mock the results
        var item = new Mock<IContentItemMigrationResult<IUser>>();
        item.Setup(item => item.ManifestEntry.Source.Location).Returns(sourceLocation);
        item.Setup(item => item.ManifestEntry.MappedLocation).Returns(destLocation);
        item.Setup(item => item.ManifestEntry.Status).Returns(MigrationManifestEntryStatus.Migrated);

        var error = new Exception("Test Exception Message");
        var errors = ImmutableList.Create(error);
        item.Setup(item => item.ManifestEntry.Errors).Returns(errors);

        var itemResults = ImmutableList.Create(item.Object);

        var result = new Mock<IContentBatchMigrationResult<IUser>>();
        result.Setup(ctx => ctx.ItemResults).Returns(itemResults);

        hook.ExecuteAsync(result.Object, CancellationToken.None);
        Assert.Single(publisher.Actions);
        Assert.Single(publisher.Messages);
        Assert.Equal(["User"], publisher.Actions);
        Assert.Contains("Could not parse error message: \nTest Exception Message", publisher.Messages[0]);
    }

    [Fact]
    public void BatchMigrationProgressHook_Execute_Errors_Formatted()
    {
        var publisher = new TestPublisher();
        var logger = new Mock<ILogger<BatchMigrationCompletedProgressHook<IUser>>>();
        var hook = new BatchMigrationCompletedProgressHook<IUser>(logger.Object, publisher);

        // Mock the results
        var item = new Mock<IContentItemMigrationResult<IUser>>();
        item.Setup(item => item.ManifestEntry.Source.Location).Returns(
            new ContentLocation(["sourceSegmentOne", "sourceSegmentTwo"]));
        item.Setup(item => item.ManifestEntry.MappedLocation).Returns(
            new ContentLocation(["destSegmentOne", "destSegmentTwo"]));
        item.Setup(item => item.ManifestEntry.Status).Returns(MigrationManifestEntryStatus.Migrated);

        var error = new Exception("URL: testurl\nCode:123\nSummary:Some summary.\nDetail:Test Error Details.");
        var errors = ImmutableList.Create(error);
        item.Setup(item => item.ManifestEntry.Errors).Returns(errors);

        var itemResults = ImmutableList.Create(item.Object);

        var result = new Mock<IContentBatchMigrationResult<IUser>>();
        result.Setup(ctx => ctx.ItemResults).Returns(itemResults);

        hook.ExecuteAsync(result.Object, CancellationToken.None);
        Assert.Single(publisher.Actions);
        Assert.Single(publisher.Messages);
        Assert.Equal(["User"], publisher.Actions);
        Assert.DoesNotContain("123", publisher.Messages[0]);
        Assert.DoesNotContain("testurl", publisher.Messages[0]);
        Assert.DoesNotContain("Some summary.", publisher.Messages[0]);
        Assert.Contains("Test Error Details", publisher.Messages[0]);
    }

    [Fact]
    public void BathMigraitonProgresHook_Execute_DifferentStatuses_No_Errors()
    {
        var publisher = new TestPublisher();
        var logger = new Mock<ILogger<BatchMigrationCompletedProgressHook<IUser>>>();
        var hook = new BatchMigrationCompletedProgressHook<IUser>(logger.Object, publisher);

        // Mock the results with different status items
        var item1 = new Mock<IContentItemMigrationResult<IUser>>();
        item1.Setup(item => item.ManifestEntry.Source.Location).Returns(
            new ContentLocation(["source1SegmentOne", "source1SegmentTwo"]));
        item1.Setup(item => item.ManifestEntry.MappedLocation).Returns(
            new ContentLocation(["dest1SegmentOne", "dest1SegmentTwo"]));
        item1.Setup(item => item.ManifestEntry.Status).Returns(MigrationManifestEntryStatus.Pending);
        item1.Setup(item => item.ManifestEntry.Errors).Returns([]);

        var item2 = new Mock<IContentItemMigrationResult<IUser>>();
        item2.Setup(item => item.ManifestEntry.Source.Location).Returns(
            new ContentLocation(["source2SegmentOne", "source2SegmentTwo"]));
        item2.Setup(item => item.ManifestEntry.MappedLocation).Returns(
            new ContentLocation(["dest2SegmentOne", "dest2SegmentTwo"]));
        item2.Setup(item => item.ManifestEntry.Status).Returns(MigrationManifestEntryStatus.Skipped);
        item2.Setup(item => item.ManifestEntry.Errors).Returns([]);

        var item3 = new Mock<IContentItemMigrationResult<IUser>>();
        item3.Setup(item => item.ManifestEntry.Source.Location).Returns(
            new ContentLocation(["source3SegmentOne", "source3SegmentTwo"]));
        item3.Setup(item => item.ManifestEntry.MappedLocation).Returns(
            new ContentLocation(["dest3SegmentOne", "dest3SegmentTwo"]));
        item3.Setup(item => item.ManifestEntry.Status).Returns(MigrationManifestEntryStatus.Migrated);
        item3.Setup(item => item.ManifestEntry.Errors).Returns([]);

        var item4 = new Mock<IContentItemMigrationResult<IUser>>();
        item4.Setup(item => item.ManifestEntry.Source.Location).Returns(
            new ContentLocation(["source4SegmentOne", "source4SegmentTwo"]));
        item4.Setup(item => item.ManifestEntry.MappedLocation).Returns(
            new ContentLocation(["dest4SegmentOne", "dest4SegmentTwo"]));
        item4.Setup(item => item.ManifestEntry.Status).Returns(MigrationManifestEntryStatus.Error);
        item4.Setup(item => item.ManifestEntry.Errors).Returns([]);

        var item5 = new Mock<IContentItemMigrationResult<IUser>>();
        item5.Setup(item => item.ManifestEntry.Source.Location).Returns(
            new ContentLocation(["source5SegmentOne", "source5SegmentTwo"]));
        item5.Setup(item => item.ManifestEntry.MappedLocation).Returns(
            new ContentLocation(["dest5SegmentOne", "dest5SegmentTwo"]));
        item5.Setup(item => item.ManifestEntry.Status).Returns(MigrationManifestEntryStatus.Canceled);
        item5.Setup(item => item.ManifestEntry.Errors).Returns([]);

        var itemResults = ImmutableList.Create(
            item1.Object,
            item2.Object,
            item3.Object,
            item4.Object,
            item5.Object);

        var result = new Mock<IContentBatchMigrationResult<IUser>>();
        result.Setup(ctx => ctx.ItemResults).Returns(itemResults);

        // Test will fail if execution throws ArgumentOutOfRangeException from statuses
        hook.ExecuteAsync(result.Object, CancellationToken.None);
    }

#pragma warning disable CS0067, SA1401, CS1696
    internal class TestPublisher : IProgressMessagePublisher
    {
        public List<string> Actions = new List<string> { };
        public List<string> Messages = new List<string> { };

        public event Action<ProgressEventArgs>? OnProgressMessage;

        public void PublishProgressMessage(string action, string message)
        {
            this.Actions.Add(action);
            this.Messages.Add(message);
            return;
        }
    }
#pragma warning restore CS0067, SA1401, CS1696
}