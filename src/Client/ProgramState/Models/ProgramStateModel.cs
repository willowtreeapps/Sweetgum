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
        /// <param name="settingsWindowPosition">The settings window position.</param>
        /// <param name="settingsWindowWidth">The settings window width.</param>
        /// <param name="settingsWindowHeight">The settings window height.</param>
        [JsonConstructor]
        public ProgramStateModel(
            IReadOnlyList<WorkbookStateModel>? workbookStates,
            PixelPoint mainWindowPosition,
            double mainWindowWidth,
            double mainWindowHeight,
            PixelPoint settingsWindowPosition,
            double settingsWindowWidth,
            double settingsWindowHeight)
        {
            this.WorkbookStates = workbookStates ?? new List<WorkbookStateModel>();
            this.MainWindowPosition = mainWindowPosition;
            this.MainWindowWidth = mainWindowWidth;
            this.MainWindowHeight = mainWindowHeight;
            this.SettingsWindowPosition = settingsWindowPosition;
            this.SettingsWindowWidth = settingsWindowWidth;
            this.SettingsWindowHeight = settingsWindowHeight;
        }

        private ProgramStateModel(ProgramStateModel source)
            : this(
                source.WorkbookStates,
                source.MainWindowPosition,
                source.MainWindowWidth,
                source.MainWindowHeight,
                source.SettingsWindowPosition,
                source.SettingsWindowWidth,
                source.SettingsWindowHeight)
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
        /// Gets the settings window position.
        /// </summary>
        public PixelPoint SettingsWindowPosition { get; private init; }

        /// <summary>
        /// Gets the settings window width.
        /// </summary>
        public double SettingsWindowWidth { get; private init; }

        /// <summary>
        /// Gets the settings window height.
        /// </summary>
        public double SettingsWindowHeight { get; private init; }

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

        /// <summary>
        /// Update the settings window details in a program state model.
        /// </summary>
        /// <param name="windowPosition">The window position.</param>
        /// <param name="windowWidth">The window width.</param>
        /// <param name="windowHeight">The window height.</param>
        /// <returns>An instance of <see cref="ProgramStateModel"/>.</returns>
        public ProgramStateModel UpdateSettingsWindow(
            PixelPoint windowPosition,
            double windowWidth,
            double windowHeight)
        {
            return new(this)
            {
                SettingsWindowPosition = windowPosition,
                SettingsWindowWidth = windowWidth,
                SettingsWindowHeight = windowHeight,
            };
        }

        /// <summary>
        /// Gets the state of a workbook by path.
        /// </summary>
        /// <param name="path">The path of the workbook.</param>
        /// <returns>An instance of <see cref="WorkbookStateModel"/>.</returns>
        public WorkbookStateModel GetWorkbookStateByPath(string path)
        {
            return this.WorkbookStates
                       .FirstOrDefault(s => s.Path == path)
                   ?? new WorkbookStateModel(
                       path,
                       new List<ExpandCollapseStateModel>(),
                       default,
                       default,
                       default);
        }
    }
}
