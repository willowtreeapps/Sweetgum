// <copyright file="WorkbookStateModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="requestStates">A list of request states for the workbook.</param>
        /// <param name="windowPosition">The workbook window position.</param>
        /// <param name="windowWidth">The workbook window width.</param>
        /// <param name="windowHeight">The workbook window height.</param>
        [JsonConstructor]
        public WorkbookStateModel(
            string? path,
            IReadOnlyList<ExpandCollapseStateModel>? expandCollapseStates,
            IReadOnlyList<RequestStateModel>? requestStates,
            PixelPoint windowPosition,
            double windowWidth,
            double windowHeight)
        {
            this.Path = path ?? string.Empty;
            this.ExpandCollapseStates = expandCollapseStates ?? new List<ExpandCollapseStateModel>();
            this.RequestStates = requestStates ?? new List<RequestStateModel>();
            this.WindowPosition = windowPosition;
            this.WindowWidth = windowWidth;
            this.WindowHeight = windowHeight;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookStateModel"/> class.
        /// </summary>
        /// <param name="source">An instance of <see cref="WorkbookStateModel"/>.</param>
        public WorkbookStateModel(WorkbookStateModel source)
            : this(source.Path, source.ExpandCollapseStates, source.RequestStates, source.WindowPosition, source.WindowWidth, source.WindowHeight)
        {
        }

        /// <summary>
        /// Gets the path to the workbook on the filesystem.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the workbook window position.
        /// </summary>
        public PixelPoint WindowPosition { get; init; }

        /// <summary>
        /// Gets the workbook window width.
        /// </summary>
        public double WindowWidth { get; init; }

        /// <summary>
        /// Gets the workbook window height.
        /// </summary>
        public double WindowHeight { get; init; }

        /// <summary>
        /// Gets a list of expand/collapse states for the workbook.
        /// </summary>
        public IReadOnlyList<ExpandCollapseStateModel> ExpandCollapseStates { get; init; }

        /// <summary>
        /// Gets a list of request states for the workbook.
        /// </summary>
        public IReadOnlyList<RequestStateModel> RequestStates { get; init; }

        /// <summary>
        /// Get the request state by request ID.
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <returns>An instance of <see cref="RequestStateModel"/>.</returns>
        public RequestStateModel GetRequestStateById(Guid requestId)
        {
            return this.RequestStates.FirstOrDefault(s => s.Id == requestId) ??
                   new RequestStateModel(requestId, default, default, default);
        }

        /// <summary>
        /// Update a request state inside the workbook state.
        /// </summary>
        /// <param name="requestStateModel">An instance of <see cref="RequestStateModel"/>.</param>
        /// <returns>An instance of <see cref="WorkbookStateModel"/>.</returns>
        public WorkbookStateModel UpdateRequest(RequestStateModel requestStateModel)
        {
            return new WorkbookStateModel(this)
            {
                RequestStates = this.RequestStates
                    .Where(s => s.Id != requestStateModel.Id)
                    .Append(requestStateModel)
                    .ToList(),
            };
        }
    }
}
