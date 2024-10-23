// <copyright file="StringIsNullOrEmptyConverter.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.Views.Converters
{
    using System;
    using System.Globalization;
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