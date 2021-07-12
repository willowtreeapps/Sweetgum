// <copyright file="ProxyOptionViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using WillowTree.Sweetgum.Client.Settings.Models;

namespace WillowTree.Sweetgum.Client.Settings.ViewModels
{
    /// <summary>
    /// The proxy option view model.
    /// </summary>
    public sealed record ProxyOptionViewModel(ProxyOption ProxyOption, string Name);
}
