// <copyright file="UserDomainMapping.axaml.cs" company="Salesforce, Inc.">
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

namespace UserDomainMappingTests;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Tableau.Migration.App.GUI.Views;
using Xunit;

public class UserDomainMappingTests
{
    [AvaloniaFact]
    public async void CheckBox_PropertyChanged_DisablesTextBox_WhenChecked()
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var view = new UserDomainMapping();
            var checkBox = view.FindControl<CheckBox>("DisableMapping");
            var textBox = view.FindControl<TextBox>("UserCloudDomain");

            Assert.NotNull(checkBox);
            checkBox!.IsChecked = true;

            Assert.NotNull(textBox);
            Assert.False(textBox!.IsEnabled);
        });
    }

    [AvaloniaFact]
    public async void CheckBox_PropertyChanged_EnablesTextBox_WhenUnchecked()
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var view = new UserDomainMapping();
            var checkBox = view.FindControl<CheckBox>("DisableMapping");
            var textBox = view.FindControl<TextBox>("UserCloudDomain");

            checkBox!.IsChecked = false;

            Assert.True(textBox!.IsEnabled);
        });
    }
}