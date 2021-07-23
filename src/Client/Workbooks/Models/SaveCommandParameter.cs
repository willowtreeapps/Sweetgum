// <copyright file="SaveCommandParameter.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using WillowTree.Sweetgum.Client.Environments.Models;
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

        /// <summary>
        /// Gets the read only list of environment models changes.
        /// </summary>
        public IReadOnlyList<EnvironmentModel>? EnvironmentModelsChanges { get; init; }
    }
}
