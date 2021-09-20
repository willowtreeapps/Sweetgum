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

            var subdirectories = Directory
                .GetDirectories(foldersPath, "*", SearchOption.AllDirectories)
                .OrderBy(p => p);

            foreach (var subdirectory in subdirectories)
            {
                var requestPath = Path.Combine(subdirectory, "request.json");
                var isRequest = File.Exists(Path.Combine(subdirectory, "request.json"));

                var relativePath = Path.GetRelativePath(foldersPath, subdirectory);
                var pathPieces = relativePath
                    .Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar })
                    .Select(Uri.UnescapeDataString);

                var pathModel = pathPieces
                    .Aggregate(
                        PathModel.Root,
                        (current, pathSegment) => current.AddSegment(pathSegment));

                if (isRequest)
                {
                    var requestJson = await File.ReadAllTextAsync(requestPath, cancellationToken);
                    var requestModel = JsonConvert.DeserializeObject<RequestModel>(requestJson)?
                        .WithNameAndParentPath(pathModel.Segments[^1], pathModel.GetParent());

                    if (requestModel == null)
                    {
                        throw new Exception("Unable to load request model from saved workbook.");
                    }

                    workbookModel = workbookModel
                        .NewRequest(pathModel)
                        .UpdateRequest(pathModel, requestModel);

                    continue;
                }

                if (!workbookModel.FolderExists(pathModel))
                {
                    workbookModel = workbookModel.NewFolder(pathModel);
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
    }
}
