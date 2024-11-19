// <copyright file="Constants.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.Views;
using System;

/// <summary>
/// Constant values used for the App Views.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Gets the maximum width for all Configuration textboxes in the app.
    /// </summary>
    public static double MaxTextboxWidth { get; } = 345.0;

    /// <summary>
    /// Gets the minimum width for the filename textbox of the UserFileMappings view.
    /// </summary>
    public static double FileMappingTextboxMinWidth { get; } = 275.0;

    /// <summary>
    /// Gets the maximum width for the filename textbox of the UserFileMappings view.
    /// </summary>
    public static double FileMappingTextboxMaxWidth { get; } = 323.0;

    /// <summary>
    /// Gets the maximum width for the message display textbox.
    /// </summary>
    public static double MessageDisplayTextboxMaxWidth { get; } = 730.0;

    // Help Texts

    /// <summary>
    /// Gets the help text for the  Migration button.
    /// </summary>
    public static string MigrationButtonHelpText { get; } =
        $@"START MIGRATION
{'\t'}will start a new migration from Tableau Server to Tableau Cloud.

RESUME MIGRATION
{'\t'}allows you to continue a previously started migration. To resume, you need to select a manifest file that contains the saved migration state. This enables the migration to continue from where it last stopped, using the progress saved in the manifest file.";

    /// <summary>
    /// Gets the templated Help text for the URI Details.
    /// </summary>
    public static string URIDetailsHelpTextTemplate { get; } =
        @"Enter the Tableau {0} URL in one of the following formats:
- For a single-site:
    http://<{1}_address>
- For a multi-site:
    http://<{1}_address>/#/site/<site_name>
The site name is parsed from the URL if one is provided.";

    /// <summary>
    /// Gets the templated Help text for the Token Details.
    /// </summary>
    public static string TokenHelpTextTemplate { get; } =
        @"Personal Access Tokens (PATs) are used for authentication.
Enter the Personal Access Token Name.
Then, provide the Personal Access Token.

Tokens can be managed in your Tableau {0} account's user settings.";

    /// <summary>
    /// Gets the help text for the User Domain Mapping view.
    /// </summary>
    public static string UserDomainMappingHelpText { get; } =
        @"Tableau Cloud usernames must be in an email format.

Enter a domain to be appended to usernames when migrating users from Tableau Server to Tableau Cloud if a user does not already have an associated email.

The domain will be used to create one in the format:
username@domain.

For example:
- 'user1' with domain `domain.com` will become
    'user1@domain.com'.
- Users with existing emails, like
    'user2@existingdomain.com', will not be affected.";

    /// <summary>
    /// Gets the help text for the User File  Mapping view.
    /// </summary>
    public static string UserFileMappingHelpText { get; } =
        "Upload a CSV file defining Tableau Server to Tableau Cloud username mappings.";
}
