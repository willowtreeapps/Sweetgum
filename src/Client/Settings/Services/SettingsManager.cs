// <copyright file="SettingsManager.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.IO;
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

        private static string GetSettingsPathAndEnsureDirectoryCreated()
        {
            var sweetgumDirectory = AppDataDirectory.GetSweetgumDirectoryAndEnsureCreated();

            return Path.Combine(sweetgumDirectory, "settings.json");
        }
    }
}
