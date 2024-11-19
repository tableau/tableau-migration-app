// <copyright file="StringIsNullOrEmptyToBooleanConverter.cs" company="Salesforce, Inc.">
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

namespace StringIsNullOrEmptyToBooleanCoverterTests;

using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using Tableau.Migration.App.GUI.Views.Converters;
using Xunit;

public class StringIsNullOrEmptyToBooleanConverterTests
{
    private readonly StringIsNullOrEmptyToBooleanConverter converter = new StringIsNullOrEmptyToBooleanConverter();

    [Fact]
    public void Convert_NullValue_ReturnsFalse()
    {
        object? value = null;
#pragma warning disable CS8605 // Disable null warning as this is what we're testing for.
        var result = this.converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.False((bool)result);
#pragma warning restore CS8605
    }

    [Fact]
    public void Convert_EmptyString_ReturnsFalse()
    {
        object? value = string.Empty;
        var result = this.converter.Convert(value!, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.False((bool)result!);
    }

    [Fact]
    public void Convert_NonEmptyString_ReturnsTrue()
    {
        object? value = "Test value";
        var result = this.converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.True((bool)result!);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        object value = true;
        Assert.Throws<NotImplementedException>(() => this.converter.ConvertBack(value, typeof(string), null, CultureInfo.InvariantCulture));
    }
}