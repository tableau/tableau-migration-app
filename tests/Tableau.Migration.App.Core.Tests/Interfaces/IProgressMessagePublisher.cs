// <copyright file="IProgressMessagePublisher.cs" company="Salesforce, Inc.">
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

namespace IProgressMessagePublisherTests;

using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.Core.Interfaces;
using Xunit;

public class IProgressMessagePublisherTests
{
    [Fact]
    public void GetStatusIcon_Pending()
    {
        Assert.Equal("\U0001F7E1", IProgressMessagePublisher.GetStatusIcon(
                         IProgressMessagePublisher.MessageStatus.Pending));
    }

    [Fact]
    public void GetStatusIcon_Skipped()
    {
        Assert.Equal("\U0001F535", IProgressMessagePublisher.GetStatusIcon(
                         IProgressMessagePublisher.MessageStatus.Skipped));
    }

    [Fact]
    public void GetStatusIcon_Error()
    {
        Assert.Equal("\U0001F534", IProgressMessagePublisher.GetStatusIcon(
                         IProgressMessagePublisher.MessageStatus.Error));
    }

    [Fact]
    public void GetStatusIcon_Succesful()
    {
        Assert.Equal("\U0001F7E2", IProgressMessagePublisher.GetStatusIcon(
                         IProgressMessagePublisher.MessageStatus.Successful));
    }

    [Fact]
    public void GetStatusIcon_Unknown()
    {
        Assert.Equal("\U0001F7E3", IProgressMessagePublisher.GetStatusIcon(
                         IProgressMessagePublisher.MessageStatus.Unknown));
    }
}