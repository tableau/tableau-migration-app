// <copyright file="IValidatableViewModel.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.ViewModels;

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