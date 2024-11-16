// <copyright file="TimerDisplayViewModel.cs" company="Salesforce, Inc.">
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
using System;
using System.ComponentModel;
using System.Diagnostics;
using Tableau.Migration.App.Core.Interfaces;

/// <summary>
/// ViewModel to manage and provide real-time timer values for both total migration
/// and action-specific elapsed time.
/// </summary>
public class TimerDisplayViewModel : INotifyPropertyChanged
{
    private readonly Stopwatch totalStopwatch = new ();
    private readonly Stopwatch actionStopwatch = new ();
    private readonly DispatcherTimer timer;
    private IProgressTimerController timerController;
    private IProgressUpdater progressUpdater;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimerDisplayViewModel"/> class,
    /// setting up and starting the timer that updates elapsed times.
    /// </summary>
    /// <param name="timerController">The object to track the migration timers from the migration service.</param>
    /// <param name="progressUpdater">The object to track the migration progress from the migration service.</param>
    public TimerDisplayViewModel(IProgressTimerController timerController, IProgressUpdater progressUpdater)
    {
        this.timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1),
        };
        this.timer.Tick += this.UpdateElapsedTimes;
        this.timer.Start();
        this.timerController = timerController;
        this.progressUpdater = progressUpdater;
        this.timerController.OnMigrationStarted += this.StartTotalTimer;
        this.timerController.OnMigrationCompleted += this.StopTotalTimer;

        this.progressUpdater.OnProgressChanged += async (sender, args) =>
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                this.OnPropertyChanged(nameof(this.ActionText));
                this.StartActionTimer();
            });
        };
    }

    /// <summary>
    /// Event for proprty changed.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the action text to display.
    /// </summary>
    public string ActionText => $"Migrating {this.progressUpdater.CurrentMigrationMessage}:";

    /// <summary>
    /// Gets the total elapsed time for the migration in hh:mm:ss format.
    /// </summary>
    public string TotalElapsedTime => $"{this.totalStopwatch.Elapsed:hh\\:mm\\:ss}";

    /// <summary>
    /// Gets the elapsed time for the current action in hh:mm:ss format.
    /// </summary>
    public string ActionElapsedTime => $"{this.actionStopwatch.Elapsed:hh\\:mm\\:ss}";

    /// <summary>
    /// Starts the total migration timer if it's not already running.
    /// </summary>
    public void StartTotalTimer()
    {
        if (!this.totalStopwatch.IsRunning)
        {
            this.totalStopwatch.Restart();
        }
    }

    /// <summary>
    /// Restarts the action timer for timing a new action.
    /// </summary>
    public void StartActionTimer()
    {
        if (this.totalStopwatch.IsRunning)
        {
            this.actionStopwatch.Restart();
        }
    }

    /// <summary>
    /// Stops the total migration timer.
    /// </summary>
    public void StopTotalTimer()
    {
        this.totalStopwatch.Stop();
        this.totalStopwatch.Reset();
        this.StopActionTimer();
    }

    /// <summary>
    /// Stops the action timer.
    /// </summary>
    public void StopActionTimer()
    {
        this.actionStopwatch.Stop();
        this.actionStopwatch.Reset();
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event to notify the UI of a property value change.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected void OnPropertyChanged(string propertyName) =>
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void UpdateElapsedTimes(object? sender, EventArgs e)
    {
        this.OnPropertyChanged(nameof(this.TotalElapsedTime));
        this.OnPropertyChanged(nameof(this.ActionElapsedTime));
    }
}
