// <copyright file="StringIsNullOrEmptyMultiConverter.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.Views.Converters;

using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// Converts a string or null to a provided displayable value.
/// </summary>
public class StringIsNullOrEmptyMultiConverter : IMultiValueConverter
{
    /// <summary>
    /// Converts a given string or null value into a string.
    /// </summary>
    /// <param name="values">The object to convert, and the value to show if null/empty.</param>
    /// <param name="targetType">The target conversion type.</param>
    /// <param name="parameter">The conversion parameters.</param>
    /// <param name="culture">The conversion culture information.</param>
    /// <returns>Either the string value if present, or else the provided default value.</returns>
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Count < 2)
        {
            return parameter?.ToString() ?? string.Empty;
        }

        var inputString = values[0] as string;
        var converterParameter = values[1] as string;

        return string.IsNullOrEmpty(inputString) ? converterParameter! : inputString!;
    }

    /// <summary>
    /// ConvertBack is not implemented.
    /// </summary>
    /// <param name="value"> The object to convert back.</param>
    /// <param name="targetTypes">The types to convert back from.</param>
    /// <param name="parameter">The conversion parameters.</param>
    /// <param name="culture">The conversion culture information.</param>
    /// <returns> The converted object.</returns>
    public IList<object?> ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}