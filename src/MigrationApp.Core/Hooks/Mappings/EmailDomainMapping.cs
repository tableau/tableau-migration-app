﻿using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings.Default;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Resources;
using Microsoft.Extensions.Options;

namespace MigrationApp.Core.Hooks.Mappings
{
    /// <summary>
    /// Mapping that appends an email domain to a username.
    /// </summary>
    public class EmailDomainMapping :
        ContentMappingBase<IUser>, // Base class to build mappings for content types
        ITableauCloudUsernameMapping
    {

        private readonly string _domain;

        /// <summary>
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
            _domain = options.Value.EmailDomain;
        }

        /// <summary>
        /// Adds an email to the user if it doesn't exist.
        /// This is where the main logic of the mapping should reside.
        /// </summary>
        public override Task<ContentMappingContext<IUser>?> MapAsync(ContentMappingContext<IUser> userMappingContext, CancellationToken cancel)
        {
            var domain = userMappingContext.MappedLocation.Parent();

            // Re-use an existing email if it already exists.
            if (!string.IsNullOrEmpty(userMappingContext.ContentItem.Email))
                return userMappingContext.MapTo(domain.Append(userMappingContext.ContentItem.Email)).ToTask();

            // Takes the existing username and appends the domain to build the email
            var email = $"{userMappingContext.ContentItem.Name}@{_domain}";
            return userMappingContext.MapTo(domain.Append(email)).ToTask();
        }
    }
}
