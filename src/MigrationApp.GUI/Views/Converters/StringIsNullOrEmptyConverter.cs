// <copyright file="StringIsNullOrEmptyConverter.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Views.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Avalonia.Data.Converters;

    /// <summary>
    /// Converts a string or null value to a displayable value.
    /// </summary>
    public class StringIsNullOrEmptyConverter : IValueConverter
    {
        /// <summary>
        /// Converts a given string or null value into a string.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <param name="targetType">The target conversion type.</param>
        /// <param name="parameter">The conversion parameters.</param>
        /// <param name="culture">The conversion culture information.</param>
        /// <returns>Either the string value if present, or else "default value".</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var input = value as string;
            var defaultMessage = parameter as string ?? "Default value"; // Use the parameter for the default message
            return string.IsNullOrEmpty(input) ? defaultMessage : input;
        }

        /// <summary>
        /// ConvertBack is not implemented.
        /// </summary>
        /// <param name="value"> The object to convert back.</param>
        /// <param name="targetType">The type to convert back from.</param>
        /// <param name="parameter">The conversion parameters.</param>
        /// <param name="culture">The conversion culture information.</param>
        /// <returns> The converted object.</returns>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}