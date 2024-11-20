// <copyright file="UriDetailsViewModel.cs" company="Salesforce, Inc.">
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

namespace UriDetailsViewModelTests;

using Avalonia.Headless.XUnit;
using Tableau.Migration.App.GUI.Models;
using Tableau.Migration.App.GUI.ViewModels;
using Xunit;

public class UriDetailsViewModelTests
{
    [AvaloniaFact]
    public void UriFull_SetValidUri_UpdatesUriBaseAndSiteContent()
    {
        var viewModel = new UriDetailsViewModel();
        var testUri = "https://example.com/#/site/mysite";

        viewModel.UriFull = testUri;

        Assert.Equal("https://example.com/", viewModel.UriBase);
        Assert.Equal("mysite", viewModel.SiteContent);
    }

    [AvaloniaFact]
    public void UriFull_SetInvalidUri_AddsUriFormatError()
    {
        var viewModel = new UriDetailsViewModel();
        var invalidUri = "invalid-uri";

        viewModel.UriFull = invalidUri;

        Assert.True(viewModel.HasErrors);
        Assert.Contains(viewModel.GetErrors(nameof(viewModel.UriFull)).Cast<string>(), e => e.Contains("Invalid URI Format"));
    }

    [AvaloniaFact]
    public void UriFull_SetEmptyUri_AddsRequiredError()
    {
        var viewModel = new UriDetailsViewModel();
        var emptyUri = string.Empty;

        viewModel.UriFull = emptyUri;

        Assert.True(viewModel.HasErrors);
        Assert.Contains(viewModel.GetErrors(nameof(viewModel.UriFull)).Cast<string>(), e => e.Contains("is required"));
    }

    [AvaloniaFact]
    public void Constructor_DefaultEnv_SetsCorrectDefaultMessage()
    {
        var viewModel = new UriDetailsViewModel();
        Assert.Equal("No Tableau Server URI has been provided.", viewModel.UriBaseDefaultMessage);
    }

    [AvaloniaFact]
    public void Constructor_CustomEnv_SetsCorrectEnvLabel()
    {
        var viewModel = new UriDetailsViewModel(TableauEnv.TableauCloud);
        Assert.Equal("No Tableau Cloud URI has been provided.", viewModel.UriBaseDefaultMessage);
    }
}