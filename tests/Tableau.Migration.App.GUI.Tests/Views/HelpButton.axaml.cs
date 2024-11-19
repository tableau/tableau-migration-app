// <copyright file="HelpButton.axaml.cs" company="Salesforce, Inc.">
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

namespace HelpButtonTests;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Tableau.Migration.App.GUI.Views;
using Xunit;

public class HelpButtonTests
{
    [AvaloniaFact]
    public void DetailsUrl_SetAndGet_ReturnsCorrectValue()
    {
        var helpButton = new HelpButton();
        string expectedUrl = "https://example.com";
        helpButton.DetailsUrl = expectedUrl;
        Assert.Equal(expectedUrl, helpButton.DetailsUrl);
    }

    [AvaloniaFact]
    public void HelpText_SetAndGet_ReturnsCorrectValue()
    {
        var helpButton = new HelpButton();
        string expectedText = "This is help text";
        helpButton.HelpText = expectedText;
        Assert.Equal(expectedText, helpButton.HelpText);
    }
}