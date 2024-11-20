// <copyright file="ViewLocator.cs" company="Salesforce, Inc.">
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

namespace ViewLocatoreTests;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using System;
using Tableau.Migration.App.GUI;
using Tableau.Migration.App.GUI.ViewModels;
using Xunit;

public class ViewLocatorTests
{
    [AvaloniaFact]
    public void Build_NullData_ReturnsNull()
    {
        var viewLocator = new ViewLocator();

        var result = viewLocator.Build(null);

        Assert.Null(result);
    }

    [AvaloniaFact]
    public void Build_UnknownViewModel_ReturnsNotFoundTextBlock()
    {
        var viewLocator = new ViewLocator();
        var unknownViewModel = new UnknownViewModel();

        var result = viewLocator.Build(unknownViewModel);

        var textBlock = Assert.IsType<TextBlock>(result);
        Assert.StartsWith("Not Found: ", textBlock.Text);
    }

    [AvaloniaFact]
    public void Match_NullData_ReturnsFalse()
    {
        var viewLocator = new ViewLocator();

        var result = viewLocator.Match(null);

        Assert.False(result);
    }

    [AvaloniaFact]
    public void Match_ValidViewModelBase_ReturnsTrue()
    {
        var viewLocator = new ViewLocator();
        var viewModelBase = new ValidViewModel();

        var result = viewLocator.Match(viewModelBase);

        Assert.True(result);
    }

    [AvaloniaFact]
    public void Match_NonViewModelBase_ReturnsFalse()
    {
        var viewLocator = new ViewLocator();
        var nonViewModelBase = new object();

        var result = viewLocator.Match(nonViewModelBase);

        Assert.False(result);
    }
}

public class UnknownViewModel : ViewModelBase
{
}

public class ValidViewModel : ViewModelBase
{
}

public class ValidView : Control
{
}