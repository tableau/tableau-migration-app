// <copyright file="EmailDomainMapping.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core.Hooks.Mappings
{
    using System.Net.Mail;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Tableau.Migration.Content;
    using Tableau.Migration.Engine.Hooks.Mappings;
    using Tableau.Migration.Engine.Hooks.Mappings.Default;
    using Tableau.Migration.Resources;

    /// <summary>
    /// Mapping that appends an email domain to a username.
    /// </summary>
    public class EmailDomainMapping :
        ContentMappingBase<IUser>, // Base class to build mappings for content types
        ITableauCloudUsernameMapping
    {
        private readonly string domain;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailDomainMapping"/> class.
        /// Creates a new <see cref="EmailDomainMapping"/> object.
        /// </summary>
        /// <param name="options">The options for this Mapping.</param>
        /// <param name="localizer">The localizer for this Mapping.</param>
        /// <param name="logger">The logger for this Mapping.</param>
        public EmailDomainMapping(
            IOptions<EmailDomainMappingOptions> options,
            ISharedResourcesLocalizer localizer,
            ILogger<EmailDomainMapping> logger)
            : base(localizer, logger)
        {
            this.domain = options.Value.EmailDomain;
        }

        /// <summary>
        /// Adds an email to the user if it doesn't exist. Migration for user will fail if username contains illegal email characters.
        /// </summary>
        /// <param name="userMappingContext">The context from which to set the user mapping information.</param>
        /// <param name="cancel">The cancellation token to interrupt the mapping process.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task<ContentMappingContext<IUser>?> MapAsync(ContentMappingContext<IUser> userMappingContext, CancellationToken cancel)
        {
            var domain = userMappingContext.MappedLocation.Parent();

            // Re-use an existing email if it already exists.
            if (!string.IsNullOrEmpty(userMappingContext.ContentItem.Email))
            {
                return userMappingContext.MapTo(domain.Append(userMappingContext.ContentItem.Email)).ToTask();
            }

            if (this.IsValidEmail(userMappingContext.ContentItem.Name))
            {
                return userMappingContext.MapTo(domain.Append(userMappingContext.ContentItem.Name)).ToTask();
            }

            // Takes the existing username and appends the domain to build the email
            var email = $"{userMappingContext.ContentItem.Name}@{this.domain}";

            // Replace spaces with `.` to reduce invalid email cases.
            email = email.Replace(' ', '.');
            return userMappingContext.MapTo(domain.Append(email)).ToTask();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}