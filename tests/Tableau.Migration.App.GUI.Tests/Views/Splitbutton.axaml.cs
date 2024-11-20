// <copyright file="Splitbutton.axaml.cs" company="Salesforce, Inc.">
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

namespace SplitButtonTests;

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Moq;
using System.Threading.Tasks;
using System.Windows.Input;
using Tableau.Migration.App.GUI.Views;
using Xunit;

public class SplitButtonTests
{
    [AvaloniaFact]
    public async Task PrimaryCommand_Should_Execute_When_Invoked()
    {
        var commandMock = new Mock<ICommand>();
        commandMock.Setup(cmd => cmd.CanExecute(It.IsAny<object>())).Returns(true);
        var splitButton = new Tableau.Migration.App.GUI.Views.SplitButton
        {
            PrimaryCommand = commandMock.Object,
        };

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            splitButton.PrimaryCommand.Execute(null);
        });

        commandMock.Verify(cmd => cmd.Execute(It.IsAny<object>()), Times.Once);
    }

    [AvaloniaFact]
    public async Task SecondaryCommand_Should_Execute_When_Invoked()
    {
        var commandMock = new Mock<ICommand>();
        commandMock.Setup(cmd => cmd.CanExecute(It.IsAny<object>())).Returns(true);
        var splitButton = new Tableau.Migration.App.GUI.Views.SplitButton
        {
            SecondaryCommand = commandMock.Object,
        };

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            splitButton.SecondaryCommand.Execute(null);
        });

        commandMock.Verify(cmd => cmd.Execute(It.IsAny<object>()), Times.Once);
    }

    [AvaloniaFact]
    public void PrimaryButtonText_Should_Default_To_Primary_Action()
    {
        var splitButton = new Tableau.Migration.App.GUI.Views.SplitButton();

        Assert.Equal("Primary Action", splitButton.PrimaryButtonText);
    }

    [AvaloniaFact]
    public void SecondaryButtonText_Should_Default_To_Secondary_Action()
    {
        var splitButton = new Tableau.Migration.App.GUI.Views.SplitButton();

        Assert.Equal("Secondary Action", splitButton.SecondaryButtonText);
    }

    [AvaloniaFact]
    public void PrimaryButtonText_Should_Be_Set_Correctly()
    {
        var splitButton = new Tableau.Migration.App.GUI.Views.SplitButton
        {
            PrimaryButtonText = "Custom Primary",
        };

        Assert.Equal("Custom Primary", splitButton.PrimaryButtonText);
    }

    [AvaloniaFact]
    public void SecondaryButtonText_Should_Be_Set_Correctly()
    {
        var splitButton = new Tableau.Migration.App.GUI.Views.SplitButton
        {
            SecondaryButtonText = "Custom Secondary",
        };

        Assert.Equal("Custom Secondary", splitButton.SecondaryButtonText);
    }
}