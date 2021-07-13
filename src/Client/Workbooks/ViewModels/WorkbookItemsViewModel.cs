// <copyright file="WorkbookItemsViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using ReactiveUI;
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
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        public WorkbookItemsViewModel(WorkbookModel workbookModel)
        {
            this.FolderItems = new WorkbookFolderItemsViewModel(workbookModel.Folders);
        }

        /// <summary>
        /// Gets the folder items.
        /// </summary>
        public WorkbookFolderItemsViewModel FolderItems { get; }
    }
}
