// <copyright file="WorkbookItemsViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Reactive;
using ReactiveUI;
using RealGoodApps.Companion.Attributes;
using WillowTree.Sweetgum.Client.Folders.Models;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.ViewModels
{
    /// <summary>
    /// A workbook items view model.
    /// </summary>
    public sealed class WorkbookItemsViewModel : ReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookItemsViewModel"/> class.
        /// </summary>
        /// <param name="folders">A read-only list of <see cref="FolderModel"/>.</param>
        /// <param name="saveCommand">A command to invoke to save the request.</param>
        public WorkbookItemsViewModel(
            IReadOnlyList<FolderModel> folders,
            ReactiveCommand<SaveCommandParameter, Unit> saveCommand)
        {
            this.FolderItems = new WorkbookFolderItemsViewModel(folders, saveCommand);
        }

        /// <summary>
        /// Gets the folder items.
        /// </summary>
        public WorkbookFolderItemsViewModel FolderItems { get; }

        /// <summary>
        /// Update the items view model by notifying the folders within the tree view to update.
        /// </summary>
        /// <param name="workbookModel">The updated workbook model.</param>
        [CompanionType(typeof(WorkbookViewModel))]
        public void Update(WorkbookModel workbookModel)
        {
            this.FolderItems.Update(workbookModel.Folders);
        }
    }
}
