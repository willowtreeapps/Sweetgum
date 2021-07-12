// <copyright file="AppDataDirectory.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace WillowTree.Sweetgum.Client
{
    /// <summary>
    /// Methods to support the local application data.
    /// </summary>
    public static class AppDataDirectory
    {
        /// <summary>
        /// Gets the "Sweetgum" directory inside the local application data folder, and ensures it has been created.
        /// </summary>
        /// <returns>The "Sweetgum" directory path.</returns>
        public static string GetSweetgumDirectoryAndEnsureCreated()
        {
            var sweetgumDirectory = Path.Combine(GetLocalAppDataFolder(), "Sweetgum");

            if (!Directory.Exists(sweetgumDirectory))
            {
                Directory.CreateDirectory(sweetgumDirectory);
            }

            return sweetgumDirectory;
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
    }
}
