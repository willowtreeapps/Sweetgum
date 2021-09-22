// <copyright file="WorkbookManager.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RealGoodApps.Companion.Attributes;
using WillowTree.Sweetgum.Client.Environments.Models;
using WillowTree.Sweetgum.Client.Folders.Models;
using WillowTree.Sweetgum.Client.Requests.Models;
using WillowTree.Sweetgum.Client.ViewModels;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.Services
{
    /// <summary>
    /// The workbook manager.
    /// </summary>
    public static class WorkbookManager
    {
        /// <summary>
        /// Load a workbook from the file system.
        /// </summary>
        /// <param name="path">The path to the workbook.</param>
        /// <param name="cancellationToken">An instance of <see cref="CancellationToken"/>.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        [CompanionType(typeof(MainWindowViewModel))]
        public static async Task<WorkbookModel> LoadAsync(
            string path,
            CancellationToken cancellationToken)
        {
            var workbookPath = Path.Combine(path, "workbook.json");
            var workbookContents = await File.ReadAllTextAsync(workbookPath, cancellationToken);
            var workbookModel = JsonConvert.DeserializeObject<WorkbookModel>(workbookContents)?.WithPath(path);

            if (workbookModel == null)
            {
                throw new Exception("Unable to load workbook.");
            }

            var foldersPath = Path.Combine(path, "folders");

            if (!Directory.Exists(foldersPath))
            {
                return workbookModel;
            }

            var entries = ScanWorkbookEntries(foldersPath);

            foreach (var entry in entries)
            {
                var subdirectory = Path.Combine(foldersPath, entry.RelativeFilesystemPath);

                if (entry.IsRequest)
                {
                    var requestPath = Path.Combine(subdirectory, "request.json");
                    var requestJson = await File.ReadAllTextAsync(requestPath, cancellationToken);
                    var requestModel = JsonConvert.DeserializeObject<RequestModel>(requestJson)?
                        .WithNameAndParentPath(entry.Path.Segments[^1], entry.Path.GetParent());

                    if (requestModel == null)
                    {
                        throw new Exception("Unable to load request model from saved workbook.");
                    }

                    workbookModel = workbookModel
                        .NewRequest(entry.Path)
                        .UpdateRequest(entry.Path, requestModel);

                    continue;
                }

                if (!workbookModel.FolderExists(entry.Path))
                {
                    var folderPath = Path.Combine(subdirectory, "folder.json");

                    var folderJson = await File.ReadAllTextAsync(folderPath, cancellationToken);
                    var folderModel = JsonConvert.DeserializeObject<FolderModel>(folderJson);

                    if (folderModel == null)
                    {
                        throw new Exception("Unable to load folder model from saved workbook.");
                    }

                    workbookModel = workbookModel
                        .NewFolder(entry.Path, folderModel.Description);
                }
            }

            return workbookModel;
        }

        /// <summary>
        /// Creates a new workbook on the file system.
        /// </summary>
        /// <param name="path">The path to the new workbook.</param>
        /// <param name="cancellationToken">An instance of <see cref="CancellationToken"/>.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        public static async Task<WorkbookModel> NewAsync(
            string path,
            CancellationToken cancellationToken)
        {
            var workbookModel = new WorkbookModel(
                "Untitled Workbook",
                path,
                new List<FolderModel>(),
                new List<EnvironmentModel>());

            await SaveAsync(workbookModel, cancellationToken);

            return workbookModel;
        }

        /// <summary>
        /// Saves a workbook on the filesystem.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        /// <param name="cancellationToken">An instance of <see cref="CancellationToken"/>.</param>
        /// <returns>An instance of <see cref="Task"/>.</returns>
        public static async Task SaveAsync(
            WorkbookModel workbookModel,
            CancellationToken cancellationToken)
        {
            if (!Directory.Exists(workbookModel.Path))
            {
                Directory.CreateDirectory(workbookModel.Path);
            }

            var workbookPath = Path.Combine(workbookModel.Path, "workbook.json");

            var foldersPath = Path.Combine(workbookModel.Path, "folders");

            if (!Directory.Exists(foldersPath))
            {
                Directory.CreateDirectory(foldersPath);
            }

            // Create a backup
            var entries = ScanWorkbookEntries(foldersPath);

            var backupPath = Path.Combine(workbookModel.Path, ".sgbackup");

            await PurgeAndCreateBackupAsync(
                backupPath,
                workbookPath,
                foldersPath,
                entries,
                cancellationToken);

            DeleteStaleWorkbook(
                workbookPath,
                foldersPath,
                entries);

            var allFolders = workbookModel.GetFlatFolders();
            foreach (var folderModel in allFolders)
            {
                var folderPath = Path.Combine(folderModel.GetPath()
                    .Segments
                    .Select(Uri.EscapeDataString)
                    .Prepend(foldersPath)
                    .ToArray());

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var folderJsonPath = Path.Combine(folderPath, "folder.json");

                await File.WriteAllTextAsync(
                    folderJsonPath,
                    JsonConvert.SerializeObject(folderModel),
                    cancellationToken);

                foreach (var requestModel in folderModel.Requests)
                {
                    var requestPath = Path.Combine(requestModel.GetPath()
                        .Segments
                        .Select(Uri.EscapeDataString)
                        .Prepend(foldersPath)
                        .ToArray());

                    if (!Directory.Exists(requestPath))
                    {
                        Directory.CreateDirectory(requestPath);
                    }

                    var requestJsonPath = Path.Combine(requestPath, "request.json");

                    await File.WriteAllTextAsync(
                        requestJsonPath,
                        JsonConvert.SerializeObject(requestModel),
                        cancellationToken);
                }
            }

            // TODO: We should probably throw an actual error if this fails.
            await File.WriteAllTextAsync(
                workbookPath,
                JsonConvert.SerializeObject(workbookModel),
                cancellationToken);
        }

        private static async Task PurgeAndCreateBackupAsync(
            string backupPath,
            string workbookPath,
            string foldersPath,
            IEnumerable<WorkbookEntry> entries,
            CancellationToken cancellationToken)
        {
            // Setup the backup directory
            if (Directory.Exists(backupPath))
            {
                Directory.Delete(backupPath, true);
            }

            Directory.CreateDirectory(backupPath);

            // Grab everything inside the workbook and folders directory to make a backup
            if (File.Exists(workbookPath))
            {
                var workbookBackupContents = await File.ReadAllTextAsync(workbookPath, cancellationToken);

                await File.WriteAllTextAsync(
                    Path.Combine(backupPath, "workbook.json"),
                    workbookBackupContents,
                    cancellationToken);
            }

            foreach (var entry in entries)
            {
                var subdirectory = Path.Combine(foldersPath, entry.RelativeFilesystemPath);
                var backupSubdirectory = Path.Combine(backupPath, "folders", entry.RelativeFilesystemPath);

                Directory.CreateDirectory(backupSubdirectory);

                if (entry.IsFolder)
                {
                    var folderBackupContents = await File.ReadAllTextAsync(
                        Path.Combine(subdirectory, "folder.json"),
                        cancellationToken);

                    await File.WriteAllTextAsync(
                        Path.Combine(backupSubdirectory, "folder.json"),
                        folderBackupContents,
                        cancellationToken);

                    continue;
                }

                var requestBackupContents = await File.ReadAllTextAsync(
                    Path.Combine(subdirectory, "request.json"),
                    cancellationToken);

                await File.WriteAllTextAsync(
                    Path.Combine(backupSubdirectory, "request.json"),
                    requestBackupContents,
                    cancellationToken);
            }
        }

        private static void DeleteStaleWorkbook(
            string workbookPath,
            string foldersPath,
            IEnumerable<WorkbookEntry> entries)
        {
            if (File.Exists(workbookPath))
            {
                File.Delete(workbookPath);
            }

            // Go backwards through the entries so we can delete them if there is nothing left in them after we remove our JSON files.
            entries = entries.Reverse();

            foreach (var entry in entries)
            {
                var subdirectory = Path.Combine(foldersPath, entry.RelativeFilesystemPath);
                var jsonPath = Path.Combine(
                    subdirectory,
                    entry.IsFolder ? "folder.json" : "request.json");

                if (File.Exists(jsonPath))
                {
                    File.Delete(jsonPath);
                }

                if (IsDirectoryEmpty(subdirectory))
                {
                    Directory.Delete(subdirectory);
                }
            }
        }

        private static List<WorkbookEntry> ScanWorkbookEntries(string foldersPath)
        {
            var workbookEntries = new List<WorkbookEntry>();
            var subdirectories = Directory
                .GetDirectories(foldersPath, "*", SearchOption.AllDirectories)
                .OrderBy(p => p);

            foreach (var subdirectory in subdirectories)
            {
                var isRequest = File.Exists(Path.Combine(subdirectory, "request.json"));
                var isFolder = File.Exists(Path.Combine(subdirectory, "folder.json"));

                var relativePath = Path.GetRelativePath(foldersPath, subdirectory);
                var pathPieces = relativePath
                    .Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar })
                    .Select(Uri.UnescapeDataString);

                var pathModel = pathPieces
                    .Aggregate(
                        PathModel.Root,
                        (current, pathSegment) => current.AddSegment(pathSegment));

                if (!isFolder && !isRequest)
                {
                    continue;
                }

                workbookEntries.Add(new WorkbookEntry(
                    relativePath,
                    pathModel,
                    isRequest,
                    isFolder));
            }

            return workbookEntries;
        }

        private static bool IsDirectoryEmpty(string path)
            => !Directory.EnumerateFileSystemEntries(path).Any();

        /// <summary>
        /// An entry for a workbook that exists on the filesystem.
        /// </summary>
        public sealed class WorkbookEntry
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WorkbookEntry"/> class.
            /// </summary>
            /// <param name="relativeFilesystemPath">The entry's relative path to the folders directory on the filesystem.</param>
            /// <param name="path">The path model for the entry.</param>
            /// <param name="isRequest">A value indicating whether or not the entry represents a request.</param>
            /// <param name="isFolder">A value indicating whether or not the entry represents a folder.</param>
            public WorkbookEntry(
                string relativeFilesystemPath,
                PathModel path,
                bool isRequest,
                bool isFolder)
            {
                this.RelativeFilesystemPath = relativeFilesystemPath;
                this.Path = path;
                this.IsRequest = isRequest;
                this.IsFolder = isFolder;
            }

            /// <summary>
            /// Gets the entry's relative path to the folders directory on the filesystem.
            /// </summary>
            public string RelativeFilesystemPath { get; }

            /// <summary>
            /// Gets the workbook entries path.
            /// </summary>
            public PathModel Path { get; }

            /// <summary>
            /// Gets a value indicating whether or not the entry represents a request.
            /// </summary>
            public bool IsRequest { get; }

            /// <summary>
            /// Gets a value indicating whether or not the entry represents a folder.
            /// </summary>
            public bool IsFolder { get; }
        }
    }
}
