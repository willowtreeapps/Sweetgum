// <copyright file="RequestStateModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using Avalonia;
using Newtonsoft.Json;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.ProgramState.Models
{
    /// <summary>
    /// The state of a request within a workbook.
    /// </summary>
    public sealed class RequestStateModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestStateModel"/> class.
        /// </summary>
        /// <param name="path">The path of the request that the state is referring to within the workbook.</param>
        /// <param name="windowPosition">The request builder window position.</param>
        /// <param name="windowWidth">The request builder window width.</param>
        /// <param name="windowHeight">The request builder window height.</param>
        [JsonConstructor]
        public RequestStateModel(
            PathModel path,
            PixelPoint windowPosition,
            double windowWidth,
            double windowHeight)
        {
            this.Path = path;
            this.WindowPosition = windowPosition;
            this.WindowWidth = windowWidth;
            this.WindowHeight = windowHeight;
        }

        /// <summary>
        /// Gets the path of the request that the state is referring to within the workbook.
        /// </summary>
        public PathModel Path { get; }

        /// <summary>
        /// Gets the request builder window position.
        /// </summary>
        public PixelPoint WindowPosition { get; }

        /// <summary>
        /// Gets the request builder window width.
        /// </summary>
        public double WindowWidth { get; }

        /// <summary>
        /// Gets the request builder window height.
        /// </summary>
        public double WindowHeight { get; }
    }
}
