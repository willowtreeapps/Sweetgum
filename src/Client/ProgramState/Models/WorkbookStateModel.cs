// <copyright file="WorkbookStateModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Avalonia;
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
        /// <param name="windowPosition">The workbook window position.</param>
        /// <param name="windowWidth">The workbook window width.</param>
        /// <param name="windowHeight">The workbook window height.</param>
        [JsonConstructor]
        public WorkbookStateModel(
            string? path,
            IReadOnlyList<ExpandCollapseStateModel>? expandCollapseStates,
            PixelPoint windowPosition,
            double windowWidth,
            double windowHeight)
        {
            this.Path = path ?? string.Empty;
            this.ExpandCollapseStates = expandCollapseStates ?? new List<ExpandCollapseStateModel>();
            this.WindowPosition = windowPosition;
            this.WindowWidth = windowWidth;
            this.WindowHeight = windowHeight;
        }

        /// <summary>
        /// Gets the path to the workbook on the filesystem.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the workbook window position.
        /// </summary>
        public PixelPoint WindowPosition { get; private init; }

        /// <summary>
        /// Gets the workbook window width.
        /// </summary>
        public double WindowWidth { get; private init; }

        /// <summary>
        /// Gets the workbook window height.
        /// </summary>
        public double WindowHeight { get; private init; }

        /// <summary>
        /// Gets a list of expand/collapse states for the workbook.
        /// </summary>
        public IReadOnlyList<ExpandCollapseStateModel> ExpandCollapseStates { get; }
    }
}
