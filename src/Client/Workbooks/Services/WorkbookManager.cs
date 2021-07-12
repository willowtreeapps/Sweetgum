// <copyright file="WorkbookManager.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RealGoodApps.Companion.Attributes;
using WillowTree.Sweetgum.Client.Folders.Models;
using WillowTree.Sweetgum.Client.ViewModels;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.Services
{
    /// <summary>
    /// The workbook manager.
    /// </summary>
    public sealed class WorkbookManager
    {
        /// <summary>
        /// Load a workbook from the file system.
        /// </summary>
        /// <param name="path">The path to the workbook.</param>
        /// <param name="cancellationToken">An instance of <see cref="CancellationToken"/>.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        [CompanionType(typeof(MainWindowViewModel))]
        public async Task<WorkbookModel> LoadAsync(
            string path,
            CancellationToken cancellationToken)
        {
            var workbookContents = await File.ReadAllTextAsync(path, cancellationToken);
            var workbookModel = JsonConvert.DeserializeObject<WorkbookModel>(workbookContents) ??
                                new WorkbookModel(string.Empty, new List<FolderModel>());

            return workbookModel;
        }

        /// <summary>
        /// Creates a new workbook on the file system.
        /// </summary>
        /// <param name="path">The path to the new workbook.</param>
        /// <param name="cancellationToken">An instance of <see cref="CancellationToken"/>.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        public async Task<WorkbookModel> NewAsync(
            string path,
            CancellationToken cancellationToken)
        {
            var workbookModel = new WorkbookModel(
                "Untitled Workbook",
                new List<FolderModel>());

            // TODO: We should probably throw an actual error if this fails.
            await File.WriteAllTextAsync(
                path,
                JsonConvert.SerializeObject(workbookModel),
                cancellationToken);

            return workbookModel;
        }
    }
}
