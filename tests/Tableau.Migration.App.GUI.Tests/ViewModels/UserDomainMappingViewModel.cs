// <copyright file="UserDomainMappingViewModel.cs" company="Salesforce, Inc.">
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

namespace UserDomainMappingViewModelTests;

using Avalonia.Headless.XUnit;
using Microsoft.Extensions.Options;
using Moq;
using Tableau.Migration.App.Core.Hooks.Mappings;
using Tableau.Migration.App.GUI.Models;
using Tableau.Migration.App.GUI.ViewModels;
using Xunit;

public class UserDomainMappingViewModelTests
{
    private Mock<IOptions<EmailDomainMappingOptions>> emailDomainOptionsMock;
    private EmailDomainMappingOptions optionsValue;
    private UserDomainMappingViewModel viewModel;

    public UserDomainMappingViewModelTests()
    {
        this.emailDomainOptionsMock = new Mock<IOptions<EmailDomainMappingOptions>>();
        this.optionsValue = new EmailDomainMappingOptions();
        this.emailDomainOptionsMock.Setup(o => o.Value).Returns(this.optionsValue);

        this.viewModel = new UserDomainMappingViewModel(this.emailDomainOptionsMock.Object);
    }

    [AvaloniaFact]
    public void ValidateDomain_ValidDomain()
    {
        this.viewModel.CloudUserDomain = "test.com";
        this.viewModel.ValidateAll();

        Assert.False(this.viewModel.HasErrors);
    }

    [AvaloniaFact]
    public void ValidateDomain_InvalidDomain()
    {
        this.viewModel.CloudUserDomain = "testcom";
        this.viewModel.ValidateAll();

        Assert.True(this.viewModel.HasErrors);
    }

    [AvaloniaFact]
    public void ValidateDomain_Optional_EmptyString()
    {
        this.viewModel.CloudUserDomain = string.Empty;
        this.viewModel.ValidateAll();

        Assert.False(this.viewModel.HasErrors);
    }
}