// <copyright file="SettingsModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

namespace WillowTree.Sweetgum.Client.Settings.Models
{
    /// <summary>
    /// A model for the settings of the application.
    /// </summary>
    public sealed record SettingsModel(string ProxyHost, int ProxyPort, ProxyOption ProxyOption);
}
