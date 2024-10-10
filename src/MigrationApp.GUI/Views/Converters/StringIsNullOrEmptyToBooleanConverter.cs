// <copyright file="StringIsNullOrEmptyToBooleanConverter.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Views.Converters
{
    using System;
    using System.Globalization;
    using Avalonia.Data.Converters;

    /// <summary>
    /// Converts a string value to a boolean based on whether it is null or empty.
    /// Returns true if a non empty string value is set.
    /// </summary>
    public class StringIsNullOrEmptyToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts the given string or null value to a boolean.
        /// Returns true if a non empty string value is set.
        /// </summary>
        /// <param name="value">The object to be converted, expected to be a string or null.</param>
        /// <param name="targetType">The target conversion type (not used in this implementation).</param>
        /// <param name="parameter">An optional parameter (not used in this implementation).</param>
        /// <param name="culture">The culture information for the conversion (not used in this implementation).</param>
        /// <returns>
        /// True if the string is not null or empty, otherwise false.
        /// </returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var input = value as string;
            return !string.IsNullOrEmpty(input);
        }

        /// <summary>
        /// Not needed for this converter.
        /// </summary>
        /// <param name="value">The object to convert back (not used).</param>
        /// <param name="targetType">The type to convert back from (not used).</param>
        /// <param name="parameter">The conversion parameter (not used).</param>
        /// <param name="culture">The culture information for the conversion (not used).</param>
        /// <returns>Throws <see cref="NotImplementedException"/> as this method is not supported.</returns>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}