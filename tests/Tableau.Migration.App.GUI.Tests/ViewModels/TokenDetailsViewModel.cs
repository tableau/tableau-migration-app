// <copyright file="TokenDetailsViewModel.cs" company="Salesforce, Inc.">
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

namespace TokenDetailsViewModel;

using Avalonia.Headless.XUnit;
using Tableau.Migration.App.GUI.Models;
using Tableau.Migration.App.GUI.ViewModels;
using Xunit;

public class TokenDetailsViewModelTests
{
    [AvaloniaFact]
    public void TokenName_Property_Should_Set_And_Get_Value()
    {
        var viewModel = new TokenDetailsViewModel();
        var tokenName = "TestTokenName";

        viewModel.TokenName = tokenName;

        Assert.Equal(tokenName, viewModel.TokenName);
    }

    [AvaloniaFact]
    public void TokenSecret_Property_Should_Set_And_Get_Value()
    {
        var viewModel = new TokenDetailsViewModel();
        var tokenSecret = "TestTokenSecret";

        viewModel.TokenSecret = tokenSecret;

        Assert.Equal(tokenSecret, viewModel.TokenSecret);
    }

    [AvaloniaFact]
    public void ValidateAll_Should_Add_ValidationErrors_When_TokenName_And_TokenSecret_Are_Empty()
    {
        var viewModel = new TokenDetailsViewModel();

        viewModel.ValidateAll();

        Assert.Contains(
            viewModel.GetErrors(nameof(viewModel.TokenName))?.Cast<object>()
            ?? Enumerable.Empty<object>(),
            error => error?.ToString()?.Contains("Access Token Name is required") ?? false);
        Assert.Contains(
            viewModel.GetErrors(nameof(viewModel.TokenSecret)).Cast<object>()
            ?? Enumerable.Empty<object>(),
            error => error.ToString()?.Contains("Access Token Secret is required") ?? false);
    }

    [AvaloniaFact]
    public void ValidateAll_Should_Not_Add_ValidationErrors_When_TokenName_And_TokenSecret_Are_Valid()
    {
        var viewModel = new TokenDetailsViewModel
        {
            TokenName = "ValidTokenName",
            TokenSecret = "ValidTokenSecret",
        };

        viewModel.ValidateAll();

        Assert.False(viewModel.HasErrors);
    }
}