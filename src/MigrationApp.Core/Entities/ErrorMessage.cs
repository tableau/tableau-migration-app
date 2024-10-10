// <copyright file="ErrorMessage.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core.Entities;

/// <summary>
/// Parsed representation of SDK Migration Error Messages.
/// </summary>
public class ErrorMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorMessage" /> class.
    /// </summary>
    /// <param name="message">The Tableau Migration SDK error message.</param>
    public ErrorMessage(string message)
    {
        this.ParseErrorMessage(message);
    }

    /// <summary>
    /// Gets or sets the URL of the error message.
    /// </summary>
    public string URL { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Code of the error message.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the summary of the error message.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the details of the error message.
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable string representation of the error.
    /// </summary>
    /// <returns>The string representation of this error message.</returns>
    public override string ToString()
    {
        return $"URL: {this.URL}\nCode: {this.Code}\nSummary: {this.Summary}\nDetail: {this.Detail}";
    }

    private void ParseErrorMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Error message cannot be null or empty.", nameof(message));
        }

        // Dictionary entry for the error message categories. Case ignored for robustness.
        Dictionary<string, string> entries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Split error message by lines, ignoring empty ones
        var lines = message.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            int colonIndex = line.IndexOf(':');

            // If the there's no category key, skip this line
            if (colonIndex <= 0)
            {
                continue;
            }

            // Split the key and value out of the line and save the entry
            string key = line.Substring(0, colonIndex).Trim();
            string val = line.Substring(colonIndex + 1).Trim();
            entries[key] = val;
        }

        this.URL = entries.GetValueOrDefault(nameof(this.URL), string.Empty);
        this.Code = entries.GetValueOrDefault(nameof(this.Code), string.Empty);
        this.Summary = entries.GetValueOrDefault(nameof(this.Summary), string.Empty);
        this.Detail = entries.GetValueOrDefault(nameof(this.Detail), string.Empty);

        if (this.Detail == string.Empty)
        {
            throw new ArgumentException("Error message received in an unknown formatting.", nameof(message));
        }
    }
}