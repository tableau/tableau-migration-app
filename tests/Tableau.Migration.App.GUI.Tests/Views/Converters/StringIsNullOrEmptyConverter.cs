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

namespace StringIsNullOrEmptyConverterTest;

using Avalonia.Data.Converters;
using System;
using System.Globalization;
using Tableau.Migration.App.GUI.Views.Converters;
using Xunit;

public class StringIsNullOrEmptyConverterTests
{
    private readonly StringIsNullOrEmptyConverter converter = new StringIsNullOrEmptyConverter();

    [Fact]
    public void Convert_NullValue_ReturnsDefaultMessage()
    {
        object? value = null;
        string expected = "Default value";
#pragma warning disable CS8605 // Disable null warning as this is what we're testing for.
        var result = this.converter.Convert(value!, typeof(string), null, CultureInfo.InvariantCulture);
#pragma warning restore CS8605

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Convert_EmptyString_ReturnsDefaultMessage()
    {
        object? value = string.Empty;
        string expected = "Default value";

        var result = this.converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Convert_StringValue_ReturnsInputValue()
    {
        object? value = "Test value";
        string expected = "Test value";

        var result = this.converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Convert_NullValue_WithParameter_ReturnsParameterMessage()
    {
        object? value = null;
        string parameter = "Custom default message";
        string expected = parameter;

        var result = this.converter.Convert(value, typeof(string), parameter, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Convert_EmptyString_WithParameter_ReturnsParameterMessage()
    {
        object? value = string.Empty;
        string parameter = "Custom default message";
        string expected = parameter;

        var result = this.converter.Convert(value, typeof(string), parameter, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        object? value = "Test value";

        Assert.Throws<NotImplementedException>(() => this.converter.ConvertBack(value, typeof(string), null, CultureInfo.InvariantCulture));
    }
}