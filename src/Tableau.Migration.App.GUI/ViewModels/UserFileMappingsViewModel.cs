// <copyright file="UserFileMappingsViewModel.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.ViewModels;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tableau.Migration.App.Core.Hooks.Mappings;
using Tableau.Migration.App.GUI.Services.Interfaces;

/// <summary>
/// ViewModel for the URI Details component.
/// </summary>
public partial class UserFileMappingsViewModel
    : ValidatableViewModelBase
{
    private readonly IOptions<DictionaryUserMappingOptions> dictionaryUserMappingOptions;
    private Dictionary<string, string> userMappings = new Dictionary<string, string>();
    private IFilePicker filePicker;
    private ICsvParser csvParser;
    private string loadedCsvFilename = "No file loaded.";
    private string csvLoadStatus = string.Empty;
    private bool isUserMappingFileLoaded = false;
    private IImmutableSolidColorBrush notificationColor = Brushes.Black;
    private IImmutableSolidColorBrush csvLoadStatusColor = Brushes.Black;
    private ILogger<UserFileMappingsViewModel>? logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserFileMappingsViewModel" /> class.
    /// </summary>
    /// <param name="dictionaryUserMappingOptions">The User Mapping Options.</param>
    /// <param name="filePicker">The filepicker.</param>
    /// <param name="csvParser">The CSV parser.</param>
    public UserFileMappingsViewModel(
        IOptions<DictionaryUserMappingOptions> dictionaryUserMappingOptions,
        IFilePicker filePicker,
        ICsvParser csvParser)
    {
        this.dictionaryUserMappingOptions = dictionaryUserMappingOptions;
        this.filePicker = filePicker;
        this.csvParser = csvParser;
        this.logger = App.ServiceProvider?
            .GetService(
                typeof(ILogger<UserFileMappingsViewModel>))
            as ILogger<UserFileMappingsViewModel>;
    }

    /// <summary>
    /// Gets or sets the loaded CSV filename used for username mappings.
    /// </summary>
    public string LoadedCSVFilename
    {
        get => this.loadedCsvFilename;
        set
        {
            if (value == string.Empty)
            {
                this.SetProperty(ref this.loadedCsvFilename, "No file loaded.");
                return;
            }

            this.SetProperty(ref this.loadedCsvFilename, value);
        }
    }

    /// <summary>
    /// Gets or sets the status message displayed after loading the user mapping file.
    /// </summary>
    public string CSVLoadStatus
    {
        get => this.csvLoadStatus;
        set => this.SetProperty(ref this.csvLoadStatus, value);
    }

    /// <summary>
    /// Gets or Sets the color to be used for the CSV load status  message.
    /// </summary>
    public IImmutableSolidColorBrush CSVLoadStatusColor
    {
        get => this.csvLoadStatusColor;
        set => this.SetProperty(ref this.csvLoadStatusColor, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether a CSV used for user mapping is loaded.
    /// </summary>
    public bool IsUserMappingFileLoaded
    {
        get => this.isUserMappingFileLoaded;
        set => this.SetProperty(ref this.isUserMappingFileLoaded, value);
    }

    /// <summary>
    /// This command executes <see cref="UnLoadUserFile" />.
    /// </summary>
    [RelayCommand]
    public void UnLoadUserFile()
    {
        this.ClearCSVLoadedValues();
        this.CSVLoadStatus = string.Empty;
        this.CSVLoadStatusColor = Brushes.Black;
    }

    /// <summary>
    /// This command executes <see cref="LoadUserFile" />.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the Load User File operation.</returns>
    [RelayCommand]
    public async Task LoadUserFile()
    {
        try
        {
            var file = await this.filePicker.OpenFilePickerAsync("Load User Mappings");

            if (file is null)
            {
                return;
            }

            string? localPath = file.TryGetLocalPath();
            if (localPath == null)
            {
                return;
            }

            try
            {
                this.userMappings = await this.csvParser.ParseAsync(localPath);
                this.dictionaryUserMappingOptions.Value.UserMappings = this.userMappings;
                this.RemoveError(nameof(this.LoadedCSVFilename), $"Failed to load file.");
                this.LoadedCSVFilename = file.Name;
                this.IsUserMappingFileLoaded = true;
                this.CSVLoadStatusColor = Brushes.Black;
                this.CSVLoadStatus = $"{this.userMappings.Count} user mappings loaded.";
            }
            catch (Exception e) when (e is InvalidDataException || e is CsvHelper.MissingFieldException || e is FileNotFoundException)
            {
                this.AddError(nameof(this.LoadedCSVFilename), $"Failed to load file.");
                this.logger?.LogInformation($"Failed to load CSV Mapping: {file.Name}.\n{e.Message}");
                this.ClearCSVLoadedValues();
            }
        }
        catch (Exception e)
        {
            this.AddError(nameof(this.LoadedCSVFilename), $"Failed to load file.");
            this.logger?.LogError($"Encountered n unknown error loading a user mapping file.\n{e}");
            this.ClearCSVLoadedValues();
        }

        return;
    }

    /// <inheritdoc/>
    public override void ValidateAll()
    {
        // Validate that a file is available if designaged that mapping has been loaded
        if (this.IsUserMappingFileLoaded)
        {
            this.ValidateRequired(this.LoadedCSVFilename, nameof(this.LoadedCSVFilename), "Failed to load file.");
        }
    }

    private void ClearCSVLoadedValues()
    {
        this.IsUserMappingFileLoaded = false;
        this.LoadedCSVFilename = string.Empty;
        this.userMappings.Clear();
        this.dictionaryUserMappingOptions.Value.UserMappings = this.userMappings;
    }
}