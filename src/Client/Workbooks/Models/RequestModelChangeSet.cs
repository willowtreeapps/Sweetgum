// <copyright file="RequestModelChangeSet.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using WillowTree.Sweetgum.Client.Requests.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.Models
{
    /// <summary>
    /// A change set of request model updates.
    /// </summary>
    public sealed class RequestModelChangeSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestModelChangeSet"/> class.
        /// </summary>
        /// <param name="originalPath">The original path of the request being changed.</param>
        /// <param name="requestModel">The updated request model.</param>
        public RequestModelChangeSet(
            PathModel originalPath,
            RequestModel requestModel)
        {
            this.OriginalPath = originalPath;
            this.RequestModel = requestModel;
        }

        /// <summary>
        /// Gets the original path.
        /// </summary>
        public PathModel OriginalPath { get; }

        /// <summary>
        /// Gets the request model.
        /// </summary>
        public RequestModel RequestModel { get; }
    }
}
