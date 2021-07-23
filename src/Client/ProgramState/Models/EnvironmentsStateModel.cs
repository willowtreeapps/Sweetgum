// <copyright file="EnvironmentsStateModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using Avalonia;
using Newtonsoft.Json;

namespace WillowTree.Sweetgum.Client.ProgramState.Models
{
    /// <summary>
    /// The state of a environments for a workbook.
    /// </summary>
    public sealed class EnvironmentsStateModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentsStateModel"/> class.
        /// </summary>
        /// <param name="windowPosition">The request builder window position.</param>
        /// <param name="windowWidth">The request builder window width.</param>
        /// <param name="windowHeight">The request builder window height.</param>
        [JsonConstructor]
        public EnvironmentsStateModel(
            PixelPoint windowPosition,
            double windowWidth,
            double windowHeight)
        {
            this.WindowPosition = windowPosition;
            this.WindowWidth = windowWidth;
            this.WindowHeight = windowHeight;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentsStateModel"/> class.
        /// </summary>
        /// <param name="source">An instance of <see cref="EnvironmentsStateModel"/>.</param>
        public EnvironmentsStateModel(EnvironmentsStateModel source)
            : this(source.WindowPosition, source.WindowWidth, source.WindowHeight)
        {
        }

        /// <summary>
        /// Gets the environment window position.
        /// </summary>
        public PixelPoint WindowPosition { get; init;  }

        /// <summary>
        /// Gets the environment window width.
        /// </summary>
        public double WindowWidth { get; init; }

        /// <summary>
        /// Gets the environment window height.
        /// </summary>
        public double WindowHeight { get; init; }
    }
}
