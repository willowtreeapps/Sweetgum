// <copyright file="ProxyOption.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

namespace WillowTree.Sweetgum.Client.Settings.Models
{
    /// <summary>
    /// An enumeration of proxy options.
    /// </summary>
    public enum ProxyOption
    {
        /// <summary>
        /// Use the system-level proxy settings.
        /// </summary>
        UseSystemSettings = 0,

        /// <summary>
        /// Manually specify proxy host/port.
        /// </summary>
        Manual,

        /// <summary>
        /// Disable proxy.
        /// </summary>
        Disabled,
    }
}
