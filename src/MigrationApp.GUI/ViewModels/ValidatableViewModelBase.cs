// <copyright file="ValidatableViewModelBase.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.ViewModels;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

/// <summary>
/// Base ViewModel with validation.
/// </summary>
public abstract class ValidatableViewModelBase : ViewModelBase, IValidatableViewModel
{
    private readonly Dictionary<string, List<string>> errors = new ();

    /// <inheritdoc/>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <inheritdoc/>
    public bool HasErrors => this.errors.Count > 0;

    /// <summary>
    /// Validates that a provided field contains a value.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="propertyName">The property name to validate.</param>
    /// <param name="errorMessage">The error message to display if validation fails.</param>
    public void ValidateRequired(
        string value,
        string propertyName,
        string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            this.AddError(propertyName, errorMessage);
        }
        else
        {
            this.RemoveError(propertyName, errorMessage);
        }
    }

    /// <inheritdoc/>
    public virtual IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName != null && this.errors.ContainsKey(propertyName))
        {
            return this.errors[propertyName];
        }

        return Array.Empty<string>();
    }

    /// <inheritdoc/>
    public void AddError(string propertyName, string error)
    {
        if (!this.errors.ContainsKey(propertyName))
        {
            this.errors[propertyName] = new List<string>();
        }

        if (!this.errors[propertyName].Contains(error))
        {
            this.errors[propertyName].Add(error);
            this.OnErrorsChanged(propertyName);
        }
    }

    /// <inheritdoc/>
    public void RemoveError(string propertyName, string error)
    {
        if (this.errors.ContainsKey(propertyName) && this.errors[propertyName].Contains(error))
        {
            this.errors[propertyName].Remove(error);
            if (!this.errors[propertyName].Any())
            {
                this.errors.Remove(propertyName);
            }

            this.OnErrorsChanged(propertyName);
        }
    }

    /// <inheritdoc/>
    public void ClearErrors(string propertyName)
    {
        if (this.errors.ContainsKey(propertyName))
        {
            this.errors.Remove(propertyName);
            this.OnErrorsChanged(propertyName);
        }
    }

    /// <inheritdoc/>
    public virtual int GetErrorCount()
    {
        return this.errors.Count;
    }

    /// <inheritdoc/>
    public abstract void ValidateAll();

    /// <summary>
    /// Trigger when errors change.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    protected void OnErrorsChanged(string propertyName)
    {
        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}