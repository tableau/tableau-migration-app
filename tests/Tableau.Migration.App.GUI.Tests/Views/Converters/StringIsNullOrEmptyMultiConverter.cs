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

namespace StringIsNullOrEmptyMultiConverterTests;

using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using Tableau.Migration.App.GUI.Views.Converters;
using Xunit;

public class StringIsNullOrEmptyMultiConverterTests
{
    private readonly StringIsNullOrEmptyMultiConverter converter = new StringIsNullOrEmptyMultiConverter();

    [Fact]
    public void Convert_NullValues_ReturnsParameterMessage()
    {
        IList<object?>? values = null;
        string parameter = "Default value";
#pragma warning disable CS8605, CS8604 // Disable null warning as this is what we're testing for.
        var result = this.converter.Convert(values, typeof(string), parameter, CultureInfo.InvariantCulture);
        Assert.Equal(parameter, result);
#pragma warning restore CS8605
    }

    [Fact]
    public void Convert_ValuesCountLessThanTwo_ReturnsParameterMessage()
    {
        IList<object?> values = new List<object?> { "Test value" };
        string parameter = "Default value";
        var result = this.converter.Convert(values, typeof(string), parameter, CultureInfo.InvariantCulture);
        Assert.Equal(parameter, result);
    }

    [Fact]
    public void Convert_NullInputString_ReturnsConverterParameter()
    {
        IList<object?> values = new List<object?> { null, "Fallback value" };
        var result = this.converter.Convert(values, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("Fallback value", result);
    }

    [Fact]
    public void Convert_EmptyInputString_ReturnsConverterParameter()
    {
        IList<object?> values = new List<object?> { string.Empty, "Fallback value" };
        var result = this.converter.Convert(values, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("Fallback value", result);
    }

    [Fact]
    public void Convert_ValidInputString_ReturnsInputString()
    {
        IList<object?> values = new List<object?> { "Valid value", "Fallback value" };
        var result = this.converter.Convert(values, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("Valid value", result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        object value = "Test value";
        Assert.Throws<NotImplementedException>(() => this.converter.ConvertBack(value, new Type[] { typeof(string) }, null, CultureInfo.InvariantCulture));
    }
}