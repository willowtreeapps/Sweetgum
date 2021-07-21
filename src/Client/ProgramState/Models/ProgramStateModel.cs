// <copyright file="ProgramStateModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
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
        [JsonConstructor]
        public ProgramStateModel(IReadOnlyList<WorkbookStateModel>? workbookStates)
        {
            this.WorkbookStates = workbookStates ?? new List<WorkbookStateModel>();
        }

        private ProgramStateModel(ProgramStateModel source)
            : this(source.WorkbookStates)
        {
        }

        /// <summary>
        /// Gets a list of workbook states.
        /// </summary>
        public IReadOnlyList<WorkbookStateModel> WorkbookStates { get; private init; }

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
    }
}
