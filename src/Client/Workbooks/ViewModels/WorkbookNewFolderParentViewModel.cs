// <copyright file="WorkbookNewFolderParentViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using ReactiveUI;
using WillowTree.Sweetgum.Client.Folders.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.ViewModels
{
    /// <summary>
    /// A workbook new folder dialog parent combobox item view model.
    /// </summary>
    public sealed class WorkbookNewFolderParentViewModel : ReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookNewFolderParentViewModel"/> class.
        /// This constructor is used for the root folder.
        /// </summary>
        public WorkbookNewFolderParentViewModel()
        {
            this.Path = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookNewFolderParentViewModel"/> class.
        /// </summary>
        /// <param name="folderModel">An instance of <see cref="FolderModel"/>.</param>
        public WorkbookNewFolderParentViewModel(FolderModel folderModel)
        {
            this.Path = folderModel.GetPath();
        }

        /// <summary>
        /// Gets the path of the parent folder.
        /// </summary>
        public string Path { get; }
    }
}
