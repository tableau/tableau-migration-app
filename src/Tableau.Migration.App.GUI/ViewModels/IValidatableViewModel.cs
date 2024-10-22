// <copyright file="IValidatableViewModel.cs" company="Salesforce, Inc.">
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

using System.ComponentModel;

/// <inheritdoc/>
public interface IValidatableViewModel : INotifyDataErrorInfo, INotifyPropertyChanged
{
    /// <summary>
    /// Adds an error message for a specific property.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="error">The error message.</param>
    void AddError(string propertyName, string error);

    /// <summary>
    /// Clears all errors for a specific property.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    void ClearErrors(string propertyName);

    /// <summary>
    /// Removes a specific error.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="error">The error message.</param>
    void RemoveError(string propertyName, string error);

    /// <summary>
    /// Validates all properties of the ViewModel.
    /// </summary>
    void ValidateAll();

    /// <summary>
    /// Gets the number of errors.
    /// </summary>
    /// <returns>Number of errors.</returns>
    int GetErrorCount();
}