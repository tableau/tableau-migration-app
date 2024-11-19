// <copyright file="EndpointOptions.cs" company="Salesforce, Inc.">
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

namespace EndpointOptions;

using Tableau.Migration.App.Core.Entities;
using Xunit;

public class EndpointOptionsTests
{
    [Fact]
    public void Defualt_constructor()
    {
        var options = new EndpointOptions();
        options.Url = new Uri("http://testurl.com");
        options.SiteContentUrl = "TestSiteContent";
        options.AccessTokenName = "TestAccessTokenName";
        options.AccessToken = "TestAccessToken";

        Assert.Equal("http://testurl.com/", options.Url.ToString());
        Assert.Equal("TestSiteContent", options.SiteContentUrl);
        Assert.Equal("TestAccessTokenName", options.AccessTokenName);
        Assert.Equal("TestAccessToken", options.AccessToken);
    }

    [Fact]
    public void Parameter_constructor()
    {
        var options = new EndpointOptions(
            "http://testurl.com",
            "TestSiteContent",
            "TestAccessTokenName",
            "TestAccessToken");

        Assert.Equal("http://testurl.com/", options.Url.ToString());
        Assert.Equal("TestSiteContent", options.SiteContentUrl);
        Assert.Equal("TestAccessTokenName", options.AccessTokenName);
        Assert.Equal("TestAccessToken", options.AccessToken);
    }

    [Fact]
    public void IsValid()
    {
        var options = new EndpointOptions();
        Assert.False(options.IsValid());

        options.Url = new Uri("http://testurl.com");
        Assert.False(options.IsValid());

        options.AccessTokenName = "TestAccessTokenName";
        Assert.False(options.IsValid());

        options.AccessToken = "TestAccessToken";

        // Not valid until URL, Token Name and Token are present.
        Assert.True(options.IsValid());
    }
}