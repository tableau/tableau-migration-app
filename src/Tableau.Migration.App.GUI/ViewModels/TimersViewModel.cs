// <copyright file="TimersViewModel.cs" company="Salesforce, Inc.">
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

using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using System;
using System.Timers;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.App.GUI.Models;

/// <summary>
/// The Viewmodel for the Timers view.
/// </summary>
public partial class TimersViewModel : ViewModelBase
{
    private DispatcherTimer dispatchTimer;
    private string totalElapsedTime = string.Empty;
    private string currentActionTime = string.Empty;
    private string currentActionLabel = string.Empty;
    private bool showActionTimer = false;
    private IMigrationTimer migrationTimer;
    private IProgressUpdater progressUpdater;
    private ILogger<TimersViewModel>? logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimersViewModel" /> class.
    /// </summary>
    /// <param name="migrationTimer">The migration timer.</param>
    /// <param name="progressUpdater">The progress Updater.</param>
    /// <param name="logger">The logger.</param>
    public TimersViewModel(
        IMigrationTimer migrationTimer,
        IProgressUpdater progressUpdater,
        ILogger<TimersViewModel>? logger)
    {
        this.migrationTimer = migrationTimer;
        this.progressUpdater = progressUpdater;
        this.logger = logger;
        this.dispatchTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1),
        };
        this.dispatchTimer.Tick += (s, e) => this.UpdateTimers();
    }

    /// <summary>
    /// Gets the Total Elapsed Time value.
    /// </summary>
    public string TotalElapsedTime
    {
        get => this.totalElapsedTime;
        private set
        {
            if (this.TotalElapsedTime != value)
            {
                this.SetProperty(ref this.totalElapsedTime, value);
            }
        }
    }

    /// <summary>
    /// Gets the Current Action Time value.
    /// </summary>
    public string CurrentActionTime
    {
        get => this.currentActionTime;
        private set
        {
            if (this.currentActionTime != value)
            {
                this.SetProperty(ref this.currentActionTime, value);
            }
        }
    }

    /// <summary>
    /// Gets the current action name.
    /// </summary>
    public string CurrentActionLabel
    {
        get => this.currentActionLabel;
        private set
        {
            if (this.currentActionLabel != value)
            {
                this.SetProperty(ref this.currentActionLabel, value);
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether gets or Sets the value indicating whether or not the timer should be running.
    /// </summary>
    public bool ShowActionTimer
    {
        get => this.showActionTimer;
        private set
        {
            if (this.showActionTimer != value)
            {
                this.SetProperty(ref this.showActionTimer, value);
            }
        }
    }

    /// <summary>
    /// Start checking for migration timer information.
    /// </summary>
    public void Start()
    {
        this.logger?.LogInformation("Timer Polling Started");
        this.TotalElapsedTime = string.Empty;
        this.dispatchTimer.Start();
    }

    /// <summary>
    /// Stop checking for migration timer information.
    /// </summary>
    public void Stop()
    {
        this.logger?.LogInformation("Timer Polling Stopped");
        this.ShowActionTimer = false;
        this.dispatchTimer.Stop();
    }

    private void UpdateTimers()
    {
        this.TotalElapsedTime = this.migrationTimer.GetTotalMigrationTime;
        this.CurrentActionTime = this.migrationTimer.GetMigrationActionTime(this.progressUpdater.CurrentMigrationStateName);
        this.CurrentActionLabel = $"{this.progressUpdater.CurrentMigrationStateName}: ";
        if (!this.showActionTimer)
        {
            this.ShowActionTimer = true;
        }

        return;
    }
}