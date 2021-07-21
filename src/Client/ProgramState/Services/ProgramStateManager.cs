// <copyright file="ProgramStateManager.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RealGoodApps.Companion.Attributes;
using WillowTree.Sweetgum.Client.ProgramState.Models;

namespace WillowTree.Sweetgum.Client.ProgramState.Services
{
    /// <summary>
    /// The program state manager.
    /// </summary>
    public sealed class ProgramStateManager
    {
        private static readonly ProgramStateModel DefaultState = new(new List<WorkbookStateModel>());

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramStateManager"/> class.
        /// </summary>
        public ProgramStateManager()
        {
            this.CurrentState = DefaultState;
        }

        /// <summary>
        /// Gets the current program state.
        /// </summary>
        public ProgramStateModel CurrentState { get; private set; }

        /// <summary>
        /// Load the program state from the file system.
        /// </summary>
        [CompanionType(typeof(App))]
        public void Load()
        {
            var programStatePath = GetProgramStatePathAndEnsureDirectoryCreated();

            if (!File.Exists(programStatePath))
            {
                this.Save(DefaultState);
                return;
            }

            // TODO: We should probably throw an actual error if this fails.
            var programStateText = File.ReadAllText(programStatePath);
            var programStateModel = JsonConvert.DeserializeObject<ProgramStateModel>(programStateText);

            if (programStateModel == null)
            {
                this.CurrentState = DefaultState;
                return;
            }

            this.CurrentState = programStateModel;
        }

        /// <summary>
        /// Save the program state to the file system.
        /// </summary>
        /// <param name="programState">An instance of <see cref="ProgramStateModel"/>.</param>
        public void Save(ProgramStateModel programState)
        {
            this.CurrentState = programState;

            // TODO: We should probably throw an actual error if this fails.
            File.WriteAllText(
                GetProgramStatePathAndEnsureDirectoryCreated(),
                JsonConvert.SerializeObject(programState));
        }

        private static string GetProgramStatePathAndEnsureDirectoryCreated()
        {
            var sweetgumDirectory = AppDataDirectory.GetSweetgumDirectoryAndEnsureCreated();

            return Path.Combine(sweetgumDirectory, "programState.json");
        }
    }
}
