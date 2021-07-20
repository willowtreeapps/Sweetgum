// <copyright file="FolderWorkbookItemViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Folders.Models;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.ViewModels
{
    /// <summary>
    /// A folder workbook item view model.
    /// </summary>
    public sealed class FolderWorkbookItemViewModel : ReactiveObject
    {
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderWorkbookItemViewModel"/> class.
        /// </summary>
        /// <param name="folderModel">An instance of <see cref="FolderModel"/>.</param>
        /// <param name="saveCommand">A command to invoke to save the request.</param>
        public FolderWorkbookItemViewModel(
            FolderModel folderModel,
            ReactiveCommand<SaveCommandParameter, Unit> saveCommand)
        {
            this.name = folderModel.Name;

            this.FolderItems = new WorkbookFolderItemsViewModel(folderModel.Folders, saveCommand);
            this.RequestItems = new WorkbookRequestItemsViewModel(folderModel.Requests, saveCommand);
        }

        /// <summary>
        /// Gets or sets the name of the folder.
        /// </summary>
        public string Name
        {
            get => this.name;
            set => this.RaiseAndSetIfChanged(ref this.name, value);
        }

        /// <summary>
        /// Gets the folder items.
        /// </summary>
        public WorkbookFolderItemsViewModel FolderItems { get; }

        /// <summary>
        /// Gets the request items.
        /// </summary>
        public WorkbookRequestItemsViewModel RequestItems { get; }
    }
}
