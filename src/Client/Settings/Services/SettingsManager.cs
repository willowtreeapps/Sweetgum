// <copyright file="SettingsManager.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using RealGoodApps.Companion.Attributes;
using WillowTree.Sweetgum.Client.Settings.Models;

namespace WillowTree.Sweetgum.Client.Settings.Services
{
    /// <summary>
    /// The settings manager.
    /// </summary>
    public sealed class SettingsManager
    {
        private static readonly SettingsModel DefaultSettings = new(
            string.Empty,
            default,
            default);

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsManager"/> class.
        /// </summary>
        public SettingsManager()
        {
            this.CurrentSettings = DefaultSettings;
        }

        /// <summary>
        /// Gets the current settings.
        /// </summary>
        public SettingsModel CurrentSettings { get; private set; }

        /// <summary>
        /// Load the settings from the file system.
        /// </summary>
        [CompanionType(typeof(App))]
        public void Load()
        {
            var settingsPath = GetSettingsPathAndEnsureDirectoryCreated();

            if (!File.Exists(settingsPath))
            {
                this.Save(DefaultSettings);
                return;
            }

            // TODO: We should probably throw an actual error if this fails.
            var settingsText = File.ReadAllText(settingsPath);
            var settingsModel = JsonConvert.DeserializeObject<SettingsModel>(settingsText);

            if (settingsModel == null)
            {
                this.CurrentSettings = DefaultSettings;
                return;
            }

            this.CurrentSettings = settingsModel;
        }

        /// <summary>
        /// Save the settings to the file system.
        /// </summary>
        /// <param name="settings">An instance of <see cref="SettingsModel"/>.</param>
        public void Save(SettingsModel settings)
        {
            this.CurrentSettings = settings;

            // TODO: We should probably throw an actual error if this fails.
            File.WriteAllText(
                GetSettingsPathAndEnsureDirectoryCreated(),
                JsonConvert.SerializeObject(settings));
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

        private static string GetSettingsPathAndEnsureDirectoryCreated()
        {
            var sweetgumDirectory = Path.Combine(GetLocalAppDataFolder(), "Sweetgum");

            if (!Directory.Exists(sweetgumDirectory))
            {
                Directory.CreateDirectory(sweetgumDirectory);
            }

            return Path.Combine(sweetgumDirectory, "settings.json");
        }
    }
}
