// <copyright file="WorkbookFolderItemsViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Folders.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.ViewModels
{
    /// <summary>
    /// A workbook folder items view model.
    /// </summary>
    public sealed class WorkbookFolderItemsViewModel : ReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookFolderItemsViewModel"/> class.
        /// </summary>
        /// <param name="folders">A read-only list of <see cref="FolderModel"/>.</param>
        public WorkbookFolderItemsViewModel(IReadOnlyList<FolderModel> folders)
        {
            this.Items = new AvaloniaList<FolderWorkbookItemViewModel>();

            // TODO: We might need a DI scope here, but this should be fine for now.
            this.Items.AddRange(folders
                .Select(f => new FolderWorkbookItemViewModel(f))
                .ToList());
        }

        /// <summary>
        /// Gets the folder items.
        /// </summary>
        public AvaloniaList<FolderWorkbookItemViewModel> Items { get; }
    }
}
