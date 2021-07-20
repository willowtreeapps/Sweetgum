// <copyright file="SaveCommandParameter.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using WillowTree.Sweetgum.Client.Requests.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.Models
{
    /// <summary>
    /// The save command parameter.
    /// </summary>
    public sealed class SaveCommandParameter
    {
        /// <summary>
        /// Gets the request model changes.
        /// </summary>
        public RequestModel? RequestModelChanges { get; init; }
    }
}
