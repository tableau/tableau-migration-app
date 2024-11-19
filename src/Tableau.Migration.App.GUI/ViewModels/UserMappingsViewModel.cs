// <copyright file="UserMappingsViewModel.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.ViewModels;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.App.Core.Hooks.Mappings;
using Tableau.Migration.App.GUI.Services.Interfaces;

/// <summary>
/// ViewModel for the URI Details component.
/// </summary>
public partial class UserMappingsViewModel
   : ValidatableViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserMappingsViewModel" /> class.
    /// </summary>
    /// <param name="userDomainMappingVM">The User Domain Mapping ViewModel.</param>
    /// <param name="userFileMappingsVM">The User File Mapping View Model.</param>
    public UserMappingsViewModel(
        UserDomainMappingViewModel userDomainMappingVM,
        UserFileMappingsViewModel userFileMappingsVM)
    {
        this.UserDomainMappingVM = userDomainMappingVM;
        this.UserFileMappingsVM = userFileMappingsVM;
    }

    /// <summary>
    /// Gets the ViewModel for User Domain Mappings.
    /// </summary>
    public UserDomainMappingViewModel UserDomainMappingVM { get; }

    /// <summary>
    /// Gets the ViewModel for User File Mappings.
    /// </summary>
    public UserFileMappingsViewModel UserFileMappingsVM { get; }

    /// <inheritdoc/>
    public override void ValidateAll()
    {
        this.UserDomainMappingVM.ValidateAll();
        this.UserFileMappingsVM.ValidateAll();
    }

    /// <inheritdoc/>
    public override int GetErrorCount()
    {
        return
            base.GetErrorCount()
            + this.UserDomainMappingVM.GetErrorCount()
            + this.UserFileMappingsVM.GetErrorCount();
    }

    /// <inheritdoc/>
    public override IEnumerable<string> GetErrors(string? propertyName)
    {
        List<string> errors = new List<string>();
        errors.AddRange(base.GetErrors(propertyName).Cast<string>());
        errors.AddRange(this.UserDomainMappingVM.GetErrors(propertyName).Cast<string>());
        errors.AddRange(this.UserFileMappingsVM.GetErrors(propertyName).Cast<string>());
        return errors;
    }
}