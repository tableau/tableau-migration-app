// <copyright file="UserMappings.axaml.cs" company="Salesforce, Inc.">
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

namespace UserMappingsTest;

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Moq;
using System.Threading.Tasks;
using System.Windows.Input;
using Tableau.Migration.App.GUI.Views;
using Xunit;

public class UserMappingsTests
{
    [AvaloniaFact]
    public void UserMappings_ctr()
    {
        var um = new UserMappings();
        Assert.NotNull(um);
    }
}