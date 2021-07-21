// <copyright file="WorkbookStateModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace WillowTree.Sweetgum.Client.ProgramState.Models
{
    /// <summary>
    /// The state of a workbook that lives on the user's filesystem.
    /// </summary>
    public sealed class WorkbookStateModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookStateModel"/> class.
        /// </summary>
        /// <param name="path">The path to the workbook on the filesystem.</param>
        /// <param name="expandCollapseStates">A list of expand/collapse states for the workbook.</param>
        [JsonConstructor]
        public WorkbookStateModel(
            string? path,
            IReadOnlyList<ExpandCollapseStateModel>? expandCollapseStates)
        {
            this.Path = path ?? string.Empty;
            this.ExpandCollapseStates = expandCollapseStates ?? new List<ExpandCollapseStateModel>();
        }

        /// <summary>
        /// Gets the path to the workbook on the filesystem.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets a list of expand/collapse states for the workbook.
        /// </summary>
        public IReadOnlyList<ExpandCollapseStateModel> ExpandCollapseStates { get; }
    }
}
