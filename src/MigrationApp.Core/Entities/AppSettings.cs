// <copyright file="AppSettings.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

/// <summary>
/// App settings to be loaded.
/// </summary>
public class AppSettings
{
    /// <summary>
    ///  Gets or Sets a value indicating whether the simulator should be used for the Tableau Migration SDK.
    /// </summary>
    public bool UseSimulator { get; set; } = false;
}