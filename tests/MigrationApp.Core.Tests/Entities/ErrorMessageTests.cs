// <copyright file="ErrorMessageTests.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ErrorMessageTests;
using MigrationApp.Core.Entities;
using System;
using Xunit;

public class ErrorMessageUnitTests
{
    [Fact]
    public void Constructor_ValidMessage_SetsAllProperties()
    {
        string message = "URL: http://example.com/error\n" +
            "Code: 404\n" +
            "Summary: Not Found\n" +
            "Detail: The requested resource was not found.";

        // Verify proper parsing
        var errorMessage = new ErrorMessage(message);

        Assert.Equal("http://example.com/error", errorMessage.URL);
        Assert.Equal("404", errorMessage.Code);
        Assert.Equal("Not Found", errorMessage.Summary);
        Assert.Equal("The requested resource was not found.", errorMessage.Detail);
    }

    [Fact]
    public void Constructor_MissingFields_SetsMissingPropertiesToEmpty()
    {
        string message = "URL: http://example.com/error\n" +
            "Detail: An unexpected error occurred.";

        // Verify proper parsing
        var errorMessage = new ErrorMessage(message);

        Assert.Equal("http://example.com/error", errorMessage.URL);
        Assert.Equal(string.Empty, errorMessage.Code);
        Assert.Equal(string.Empty, errorMessage.Summary);
        Assert.Equal("An unexpected error occurred.", errorMessage.Detail);
    }

    [Fact]
    public void Constructor_EmptyMessage_ThrowsArgumentException()
    {
        string message = string.Empty;

        // Throws on Empty message
        var exception = Assert.Throws<ArgumentException>(() => new ErrorMessage(message));
        Assert.Contains("Error message cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void Constructor_NullMessage_ThrowsArgumentException()
    {
        string? message = null;

        // Throws on null message
        var exception = Assert.Throws<ArgumentException>(() => new ErrorMessage(message!));
        Assert.Contains("Error message cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void Constructor_InvalidFormat_ThrowsArgumentException()
    {
        string message = "Differently formatted\n message that can't be parsed.";

        // Throw for unknown formatting
        var exception = Assert.Throws<ArgumentException>(() => new ErrorMessage(message));
        Assert.Contains("Error message received in an unknown formatting.", exception.Message);
    }

    [Fact]
    public void Constructor_CaseInsensitiveKeys_SetsPropertiesCorrectly()
    {
        string message = "url: http://example.com/error\n" +
            "CODE: 500\n" +
            "summary: Internal Server Error\n" +
            "DETAIL: An unexpected error occurred on the server.";

        // Verify that different case doesn't affect the parsing
        var errorMessage = new ErrorMessage(message);

        Assert.Equal("http://example.com/error", errorMessage.URL);
        Assert.Equal("500", errorMessage.Code);
        Assert.Equal("Internal Server Error", errorMessage.Summary);
        Assert.Equal("An unexpected error occurred on the server.", errorMessage.Detail);
    }

    [Fact]
    public void Constructor_ExtraWhitespace_TrimsValues()
    {
        string message = "URL:    http://example.com/error   \n" +
            "Code:  403\n" +
            "Summary: Forbidden  \n" +
            "Detail:   You do not have permission to access this resource.  ";

        // Verify that error values are trimmed for legibility
        var errorMessage = new ErrorMessage(message);

        Assert.Equal("http://example.com/error", errorMessage.URL);
        Assert.Equal("403", errorMessage.Code);
        Assert.Equal("Forbidden", errorMessage.Summary);
        Assert.Equal("You do not have permission to access this resource.", errorMessage.Detail);
    }

    [Fact]
    public void Constructor_MultipleSameKeys_UseLast()
    {
        string message = "Code: 400\n" +
        "Code: 401\n" +
        "Detail: Initial detail.\n" +
        "Detail: Updated detail.";

        // When multiple of the same key are provided, use the last one seen
        var errorMessage = new ErrorMessage(message);

        Assert.Equal(string.Empty, errorMessage.URL);
        Assert.Equal("401", errorMessage.Code);
        Assert.Equal(string.Empty, errorMessage.Summary);
        Assert.Equal("Updated detail.", errorMessage.Detail);
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        string message = "URL: http://example.com/error\n" +
            "Code: 404\n" +
            "Summary: Not Found\n" +
            "Detail: The requested resource was not found.";

        // Verify that the ToString() should output back the original message
        var errorMessage = new ErrorMessage(message);
        string result = errorMessage.ToString();

        Assert.Equal(message, result);
    }

    [Fact]
    public void Constructor_MessageWithExtraLines_IgnoresInvalidLines()
    {
        string message = "URL: http://example.com/error\n" +
            "Some random text without colon\n" +
            "Code: 500\n" +
            "Another invalid line\n" +
            "Detail: Server error occurred.";

        // Lines not following the key:value format and aren't one of the defined entries will be ignored
        var errorMessage = new ErrorMessage(message);

        Assert.Equal("http://example.com/error", errorMessage.URL);
        Assert.Equal("500", errorMessage.Code);
        Assert.Equal(string.Empty, errorMessage.Summary);
        Assert.Equal("Server error occurred.", errorMessage.Detail);
    }

    [Fact]
    public void Constructor_MessageWithOnlyDetail_SetsOnlyDetail()
    {
        string message = "Detail: Only detail provided.";

        var errorMessage = new ErrorMessage(message);

        Assert.Equal(string.Empty, errorMessage.URL);
        Assert.Equal(string.Empty, errorMessage.Code);
        Assert.Equal(string.Empty, errorMessage.Summary);
        Assert.Equal("Only detail provided.", errorMessage.Detail);
    }

    [Fact]
    public void Constructor_DetailIsEmptyAfterParsing_ThrowsArgumentException()
    {
        string message = "URL: http://example.com/error\n" +
            "Code: 400\n" +
            "Summary: Bad Request\n" +
            "Detail:    "; // Detail is empty after trimming

        // Verify that detail must exist
        var exception = Assert.Throws<ArgumentException>(() => new ErrorMessage(message));
        Assert.Equal("Error message received in an unknown formatting. (Parameter 'message')", exception.Message);
    }
}