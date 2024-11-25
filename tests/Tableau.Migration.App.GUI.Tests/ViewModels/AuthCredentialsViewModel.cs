// <copyright file="AuthCredentialsViewModel.cs" company="Salesforce, Inc.">
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

namespace AuthCredentialsViewModelTests;

using Avalonia.Headless.XUnit;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.App.GUI.Models;
using Tableau.Migration.App.GUI.ViewModels;
using Xunit;

public class AuthCredentialsViewModelTests
{
    [AvaloniaFact]
    public void Constructor_InitializesUriAndTokenDetailsViewModels()
    {
        var viewModel = new AuthCredentialsViewModel();
        Assert.NotNull(viewModel.UriDetailsVM);
        Assert.NotNull(viewModel.TokenDetailsVM);
    }
}