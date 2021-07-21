// <copyright file="ProgramStateModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Newtonsoft.Json;

namespace WillowTree.Sweetgum.Client.ProgramState.Models
{
    /// <summary>
    /// The state of the program that is stored automatically on the user's file system.
    /// This contains stuff like the workbook expand/collapse states for folders.
    /// That way, when the user closes a workbook and then loads it later, they feel like they are back where they left off.
    /// </summary>
    public sealed class ProgramStateModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramStateModel"/> class.
        /// </summary>
        /// <param name="workbookStates">A list of workbook states.</param>
        /// <param name="mainWindowPosition">The main window position.</param>
        /// <param name="mainWindowWidth">The main window width.</param>
        /// <param name="mainWindowHeight">The main window height.</param>
        [JsonConstructor]
        public ProgramStateModel(
            IReadOnlyList<WorkbookStateModel>? workbookStates,
            PixelPoint mainWindowPosition,
            double mainWindowWidth,
            double mainWindowHeight)
        {
            this.WorkbookStates = workbookStates ?? new List<WorkbookStateModel>();
            this.MainWindowPosition = mainWindowPosition;
            this.MainWindowWidth = mainWindowWidth;
            this.MainWindowHeight = mainWindowHeight;
        }

        private ProgramStateModel(ProgramStateModel source)
            : this(source.WorkbookStates, source.MainWindowPosition, source.MainWindowWidth, source.MainWindowHeight)
        {
        }

        /// <summary>
        /// Gets a list of workbook states.
        /// </summary>
        public IReadOnlyList<WorkbookStateModel> WorkbookStates { get; private init; }

        /// <summary>
        /// Gets the main window position.
        /// </summary>
        public PixelPoint MainWindowPosition { get; private init; }

        /// <summary>
        /// Gets the main window width.
        /// </summary>
        public double MainWindowWidth { get; private init; }

        /// <summary>
        /// Gets the main window height.
        /// </summary>
        public double MainWindowHeight { get; private init; }

        /// <summary>
        /// Update a workbook state in a program state model.
        /// </summary>
        /// <param name="workbookStateModel">The update workbook state.</param>
        /// <returns>An instance of <see cref="ProgramStateModel"/>.</returns>
        public ProgramStateModel UpdateWorkbook(WorkbookStateModel workbookStateModel)
        {
            return new(this)
            {
                WorkbookStates = this.WorkbookStates
                    .Where(s => s.Path != workbookStateModel.Path)
                    .Append(workbookStateModel)
                    .ToList(),
            };
        }

        /// <summary>
        /// Update the main window details in a program state model.
        /// </summary>
        /// <param name="windowPosition">The window position.</param>
        /// <param name="windowWidth">The window width.</param>
        /// <param name="windowHeight">The window height.</param>
        /// <returns>An instance of <see cref="ProgramStateModel"/>.</returns>
        public ProgramStateModel UpdateMainWindow(
            PixelPoint windowPosition,
            double windowWidth,
            double windowHeight)
        {
            return new(this)
            {
                MainWindowPosition = windowPosition,
                MainWindowWidth = windowWidth,
                MainWindowHeight = windowHeight,
            };
        }
    }
}
