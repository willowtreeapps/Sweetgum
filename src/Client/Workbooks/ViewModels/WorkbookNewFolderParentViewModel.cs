// <copyright file="WorkbookNewFolderParentViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Folders.Models;
using WillowTree.Sweetgum.Client.Workbooks.Models;

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
            : this(PathModel.Root)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookNewFolderParentViewModel"/> class.
        /// </summary>
        /// <param name="folderModel">An instance of <see cref="FolderModel"/>.</param>
        public WorkbookNewFolderParentViewModel(FolderModel folderModel)
            : this(folderModel.GetPath())
        {
        }

        private WorkbookNewFolderParentViewModel(PathModel pathModel)
        {
            this.Path = pathModel;
            this.DisplayPath = pathModel.IsRoot() ? "<root>" : pathModel.Segments[^1];
            this.DepthWidth = Math.Max(0, (pathModel.Segments.Count - 1) * 20);
        }

        /// <summary>
        /// Gets the folder path.
        /// </summary>
        public PathModel Path { get; }

        /// <summary>
        /// Gets the depth rectangle width.
        /// </summary>
        public double DepthWidth { get; }

        /// <summary>
        /// Gets the display path of the folder.
        /// </summary>
        public string DisplayPath { get; }
    }
}
