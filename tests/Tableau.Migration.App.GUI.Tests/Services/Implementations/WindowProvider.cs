// <copyright file="WindowProvider.cs" company="Salesforce, Inc.">
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

namespace WindowProviderTests;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using System;
using Tableau.Migration.App.GUI.Services.Implementations;
using Tableau.Migration.App.GUI.Tests;
using Xunit;

public class WindowProviderTests
{
    [AvaloniaFact]
    public void GetMainWindow_ShouldReturnMainWindow_WhenApplicationLifetimeIsClassicDesktop()
    {
        var appBuilder = AppBuilder.Configure<TestApp>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
        var lifetime = new ClassicDesktopStyleApplicationLifetime { MainWindow = new Window() };
        appBuilder.SetupWithLifetime(lifetime);

        var windowProvider = new WindowProvider();
        var mainWindow = windowProvider.GetMainWindow();

        Assert.NotNull(mainWindow);
        Assert.Equal(lifetime.MainWindow, mainWindow);
    }

    [AvaloniaFact(Skip = "AppBuilder singleton conflict between tests. Skipped until that is resolved.")]
    public void GetMainWindow_ShouldReturnMainWindow_2()
    {
        var appBuilder = AppBuilder.Configure<TestApp>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
        var lifetime = new ClassicDesktopStyleApplicationLifetime();
        appBuilder.SetupWithLifetime(lifetime);

        var windowProvider = new WindowProvider();

        Assert.Throws<InvalidOperationException>(() => windowProvider.GetMainWindow());
    }
}