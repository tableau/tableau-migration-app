// <copyright file="UserFileMappingsViewModel.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.ViewModels;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using MigrationApp.Core.Hooks.Mappings;
using MigrationApp.GUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// ViewModel for the URI Details component.
/// </summary>
public partial class UserFileMappingsViewModel
    : ViewModelBase
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

    [RelayCommand]
    private void UnLoadUserFile()
    {
        this.ClearCSVLoadedValues();
        this.CSVLoadStatus = string.Empty;
        this.CSVLoadStatusColor = Brushes.Black;
    }

    [RelayCommand]
    private async Task LoadUserFile()
    {
        try
        {
            var file = await this.filePicker.OpenFilePickerAsync("Load User Mappings");

            if (file is null)
            {
                this.CSVLoadStatusColor = Brushes.Red;
                this.CSVLoadStatus = "Could not find file.";
                return;
            }

            string? localPath = file.TryGetLocalPath();
            if (localPath == null)
            {
                this.CSVLoadStatusColor = Brushes.Red;
                this.CSVLoadStatus = "Could not find file.";
                return;
            }

            try
            {
                this.userMappings = await this.csvParser.ParseAsync(localPath);
                this.dictionaryUserMappingOptions.Value.UserMappings = this.userMappings;
                this.LoadedCSVFilename = file.Name;
                this.IsUserMappingFileLoaded = true;
                this.CSVLoadStatusColor = Brushes.Black;
                this.CSVLoadStatus = $"{this.userMappings.Count} user mappings loaded.";
            }
            catch (InvalidDataException e)
            {
                this.CSVLoadStatusColor = Brushes.Red;
                this.CSVLoadStatus = $"Failed to load {file.Name}.\n{e.Message}";
                this.ClearCSVLoadedValues();
            }
            catch (FileNotFoundException)
            {
                this.CSVLoadStatusColor = Brushes.Red;
                this.CSVLoadStatus = "Could not find file.";
                this.CSVLoadStatusColor = Brushes.Red;
                this.ClearCSVLoadedValues();
            }
        }
        catch (Exception e)
        {
            this.CSVLoadStatusColor = Brushes.Red;
            this.CSVLoadStatus = $"Encountered an unexpected error with file picker.\n{e.Message}";
            this.CSVLoadStatusColor = Brushes.Red;
            this.ClearCSVLoadedValues();
        }

        return;
    }

    private void ClearCSVLoadedValues()
    {
        this.IsUserMappingFileLoaded = false;
        this.LoadedCSVFilename = string.Empty;
        this.userMappings.Clear();
        this.dictionaryUserMappingOptions.Value.UserMappings = this.userMappings;
    }
}