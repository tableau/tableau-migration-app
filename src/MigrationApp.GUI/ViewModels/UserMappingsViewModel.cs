// <copyright file="UserMappingsViewModel.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.ViewModels;
using Microsoft.Extensions.Options;
using MigrationApp.Core.Hooks.Mappings;
using MigrationApp.GUI.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ViewModel for the URI Details component.
/// </summary>
public partial class UserMappingsViewModel
   : ValidatableViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserMappingsViewModel" /> class.
    /// </summary>
    /// <param name="emailOptions">Email Domain options to be used for setting Default domain hook.</param>
    /// <param name="dictionaryUserMappingOptions">The User Mapping Options.</param>
    /// <param name="filePicker">The filepicker.</param>
    /// <param name="csvParser">The CSV parser.</param>
    public UserMappingsViewModel(
        IOptions<EmailDomainMappingOptions> emailOptions,
        IOptions<DictionaryUserMappingOptions> dictionaryUserMappingOptions,
        IFilePicker filePicker,
        ICsvParser csvParser)
    {
        this.UserDomainMappingVM = new UserDomainMappingViewModel(emailOptions);
        this.UserFileMappingsVM = new UserFileMappingsViewModel(
            dictionaryUserMappingOptions,
            filePicker,
            csvParser);
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
    }

    /// <inheritdoc/>
    public override int GetErrorCount()
    {
        return
            base.GetErrorCount()
            + this.UserDomainMappingVM.GetErrorCount();
    }

    /// <inheritdoc/>
    public override IEnumerable<string> GetErrors(string? propertyName)
    {
        List<string> errors = new List<string>();
        errors.AddRange(base.GetErrors(propertyName).Cast<string>());
        errors.AddRange(this.UserDomainMappingVM.GetErrors(propertyName).Cast<string>());
        return errors;
    }
}