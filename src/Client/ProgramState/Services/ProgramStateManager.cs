// <copyright file="ProgramStateManager.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
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

        /// <summary>
        /// .NET Core gives you access to a directory that you can write local application data to, but for some reason
        /// it does not use the standard OSX location. This method will construct a path to the user's local application data,
        /// while taking this quirk into account. On Mac OSX, it will fall back to the .NET Core non-standard directory
        /// if there is no way to determine the conventional path.
        /// </summary>
        /// <returns>The local app data folder.</returns>
        private static string GetLocalAppDataFolder()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return localAppData;
            }

            var homeDirectory = Environment.GetEnvironmentVariable("HOME");

            if (string.IsNullOrWhiteSpace(homeDirectory))
            {
                return localAppData;
            }

            return Path.Combine(homeDirectory, "Library", "Application Support");
        }

        private static string GetProgramStatePathAndEnsureDirectoryCreated()
        {
            var sweetgumDirectory = Path.Combine(GetLocalAppDataFolder(), "Sweetgum");

            if (!Directory.Exists(sweetgumDirectory))
            {
                Directory.CreateDirectory(sweetgumDirectory);
            }

            return Path.Combine(sweetgumDirectory, "programState.json");
        }
    }
}
