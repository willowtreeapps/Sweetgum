// <copyright file="ExpandCollapseStateModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using Newtonsoft.Json;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.ProgramState.Models
{
    /// <summary>
    /// State that encapsulates whether or not a folder within a workbook is expanded or collapsed.
    /// </summary>
    public sealed class ExpandCollapseStateModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandCollapseStateModel"/> class.
        /// </summary>
        /// <param name="folderPath">An instance of <see cref="PathModel"/>.</param>
        /// <param name="isExpanded">A value indicating whether or not the folder is expanded.</param>
        [JsonConstructor]
        public ExpandCollapseStateModel(
            PathModel? folderPath,
            bool isExpanded)
        {
            this.FolderPath = folderPath ?? PathModel.Root;
            this.IsExpanded = isExpanded;
        }

        /// <summary>
        /// Gets the folder path.
        /// </summary>
        public PathModel FolderPath { get; init; }

        /// <summary>
        /// Gets a value indicating whether or not the folder is expanded.
        /// </summary>
        public bool IsExpanded { get; init; }
    }
}
