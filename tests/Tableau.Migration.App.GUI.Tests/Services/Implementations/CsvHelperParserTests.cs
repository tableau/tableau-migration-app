// <copyright file="CsvHelperParserTests.cs" company="Salesforce, Inc.">
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

namespace MigrationApp.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Tableau.Migration.App.GUI.Services.Implementations;
    using Tableau.Migration.App.GUI.Services.Interfaces;
    using Xunit;

    public class CsvHelperParserTests : IDisposable
    {
        private readonly string tempFilePath;

        public CsvHelperParserTests()
        {
            // Create a temporary file to be used in each test
            this.tempFilePath = Path.GetTempFileName();
        }

        [Fact]
        public void Dispose()
        {
            // Clean up the temporary file after each test
            if (File.Exists(this.tempFilePath))
            {
                File.Delete(this.tempFilePath);
            }
        }

        [Fact]
        public async Task ParseAsync_ValidCsv_ReturnsCorrectDictionary()
        {
            // Setup
            var csvContent = "serverUser1,cloudUser1@host1.com\nserverUser2,cloudUser2@host2.com\nserverUser3,cloudUser3@host3.com";
            await File.WriteAllTextAsync(this.tempFilePath, csvContent);

            ICsvParser parser = new CsvHelperParser();

            var expected = new Dictionary<string, string>
            {
                { "serverUser1", "cloudUser1@host1.com" },
                { "serverUser2", "cloudUser2@host2.com" },
                { "serverUser3", "cloudUser3@host3.com" },
            };

            // Parse File
            var result = await parser.ParseAsync(this.tempFilePath);

            // Assert
            Assert.Equal(expected.Count, result.Count);
            foreach (var userEntry in expected)
            {
                Assert.True(result.ContainsKey(userEntry.Key), $"Result does not contain key '{userEntry.Key}'.");
                Assert.Equal(userEntry.Value, result[userEntry.Key]);
            }
        }

        [Fact]
        public async Task ParseAsync_CsvWithExtraColumns_ThrowsInvalidDataException()
        {
            // Setup data contianing row with extra column
            var csvContent = "serverUser1,cloudUser1@host1.com,extraColumn\nserverUser2,cloudUser2@host2.com";
            await File.WriteAllTextAsync(this.tempFilePath, csvContent);

            ICsvParser parser = new CsvHelperParser();

            // Attempt to Parse File
            await Assert.ThrowsAsync<InvalidDataException>(() => parser.ParseAsync(this.tempFilePath));
        }

        [Fact]
        public async Task ParseAsync_CsvWithInvalidEmail_ThrowsInvalidDataException()
        {
            // Setup data contianing row with extra column
            var csvContent = "serverUser1,cloudUser1@host1.com,nserverUser2,cloudUser2@host2";
            await File.WriteAllTextAsync(this.tempFilePath, csvContent);

            ICsvParser parser = new CsvHelperParser();

            // Attempt to Parse File
            await Assert.ThrowsAsync<InvalidDataException>(() => parser.ParseAsync(this.tempFilePath));
        }

        [Fact]
        public async Task ParseAsync_EmptyCsv_ReturnsEmptyDictionary()
        {
            // Setup
            var csvContent = string.Empty;
            await File.WriteAllTextAsync(this.tempFilePath, csvContent);

            ICsvParser parser = new CsvHelperParser();

            // Parse Empty File
            var result = await parser.ParseAsync(this.tempFilePath);

            // Expect an empty dictionary back
            Assert.Empty(result);
        }

        [Fact]
        public async Task ParseAsync_DuplicateServerUsername_OverwritesWithLatestCloudUsername()
        {
            // Setup dicitonary with multiple entries for the same user
            var csvContent = "serverUser1,cloudUser1@host1.com\nserverUser2,cloudUser2@host2.com\nserverUser1,cloudUser3@host3.com";
            await File.WriteAllTextAsync(this.tempFilePath, csvContent);

            ICsvParser parser = new CsvHelperParser();

            var expected = new Dictionary<string, string>
            {
                { "serverUser1", "cloudUser3@host3.com" }, // Overwritten
                { "serverUser2", "cloudUser2@host2.com" },
            };

            // Parse File
            var result = await parser.ParseAsync(this.tempFilePath);

            // Verify that only the last entry was used
            Assert.Equal(expected.Count, result.Count);
            foreach (var userEntry in expected)
            {
                Assert.True(result.ContainsKey(userEntry.Key), $"Result does not contain key '{userEntry.Key}'.");
                Assert.Equal(userEntry.Value, result[userEntry.Key]);
            }
        }

        [Fact]
        public async Task ParseAsync_InvalidCSV_MissingColumn_ThrowsInvalidData()
        {
            // Setup invalid CSV with invalid rows containing only one value
            var csvContent = "serverUser1,cloudUser1@host1.com\n,cloudUser2\nserverUser3@host3.com,\nserverUser4,cloudUser4@host4.com";
            await File.WriteAllTextAsync(this.tempFilePath, csvContent);

            ICsvParser parser = new CsvHelperParser();

            // Attempt to parse file
            await Assert.ThrowsAsync<InvalidDataException>(() => parser.ParseAsync(this.tempFilePath));
        }

        [Fact]
        public async Task ParseAsync_InvalidCSV_NonEmailCloudUsernames_ThrowsInvalidData()
        {
            // Setup invalid CSV with invalid rows containing only one value
            var csvContent = "serverUser1,cloudUser1@host1.com\n,serverUser2,cloudUser2\nserverUser3@host3.com,\nserverUser4,cloudUser4@host4.com";
            await File.WriteAllTextAsync(this.tempFilePath, csvContent);

            ICsvParser parser = new CsvHelperParser();

            // Attempt to parse file
            await Assert.ThrowsAsync<InvalidDataException>(() => parser.ParseAsync(this.tempFilePath));
        }

        [Fact]
        public async Task ParseAsync_FileDoesNotExist_ThrowsFileNotFoundException()
        {
            // Setup path to non existent file
            var nonExistentFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".csv");
            ICsvParser parser = new CsvHelperParser();

            // Attempt to parse file
            Assert.False(File.Exists(nonExistentFilePath), $"Test setup error: The file '{nonExistentFilePath}' unexpectedly exists.");
            await Assert.ThrowsAsync<FileNotFoundException>(() => parser.ParseAsync(nonExistentFilePath));
        }

        [Fact]
        public async Task ParseAsync_CsvWithWhitespace_ParsesCorrectly()
        {
            // Setup files with leading and trailing spaces in usernames
            var csvContent = " serverUser1 , cloudUser1@host1.com \nserverUser2,cloudUser2@host2.com";
            await File.WriteAllTextAsync(this.tempFilePath, csvContent);

            ICsvParser parser = new CsvHelperParser();

            var expected = new Dictionary<string, string>
            {
                { "serverUser1", "cloudUser1@host1.com" }, // Whitespaces are trimmed
                { "serverUser2", "cloudUser2@host2.com" },
            };

            // Parse file
            var result = await parser.ParseAsync(this.tempFilePath);

            // Verify that parsed usernames are trimmed
            Assert.Equal(expected.Count, result.Count);
            foreach (var kvp in expected)
            {
                Assert.True(result.ContainsKey(kvp.Key), $"Result does not contain key '{kvp.Key}'.");
                Assert.Equal(kvp.Value, result[kvp.Key]);
            }
        }
    }
}