// <copyright file="WorkbookItemsViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Folders.Models;

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
        public WorkbookItemsViewModel(IReadOnlyList<FolderModel> folders)
        {
            this.FolderItems = new WorkbookFolderItemsViewModel(folders);
        }

        /// <summary>
        /// Gets the folder items.
        /// </summary>
        public WorkbookFolderItemsViewModel FolderItems { get; }
    }
}
