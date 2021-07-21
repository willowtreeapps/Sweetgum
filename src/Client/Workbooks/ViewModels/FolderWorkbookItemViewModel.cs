// <copyright file="FolderWorkbookItemViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
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
        private readonly ObservableAsPropertyHelper<string> expandCollapseTextObservableAsPropertyHelper;
        private bool isExpanded;
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
            this.isExpanded = false;
            this.name = folderModel.Name;
            this.Path = folderModel.GetPath();

            this.FolderItems = new WorkbookFolderItemsViewModel(folderModel.Folders, saveCommand);
            this.RequestItems = new WorkbookRequestItemsViewModel(folderModel.Requests, saveCommand);

            this.ToggleExpandCollapseCommand = ReactiveCommand.Create(() =>
            {
                this.IsExpanded = !this.IsExpanded;
            });

            this.expandCollapseTextObservableAsPropertyHelper = this.WhenAnyValue(viewModel => viewModel.IsExpanded)
                .Select(currentIsExpanded => currentIsExpanded ? "-" : "+")
                .ToProperty(this, viewModel => viewModel.ExpandCollapseText);
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
        /// Gets or sets a value indicating whether or not the folder is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.RaiseAndSetIfChanged(ref this.isExpanded, value);
        }

        /// <summary>
        /// Gets the expand/collapse button text.
        /// </summary>
        public string ExpandCollapseText => this.expandCollapseTextObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets a command to toggle the expand/collapse state.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ToggleExpandCollapseCommand { get; }

        /// <summary>
        /// Gets the folder items.
        /// </summary>
        public WorkbookFolderItemsViewModel FolderItems { get; }

        /// <summary>
        /// Gets the request items.
        /// </summary>
        public WorkbookRequestItemsViewModel RequestItems { get; }

        /// <summary>
        /// Gets the path model for the folder.
        /// </summary>
        public PathModel Path { get; }

        /// <summary>
        /// This will let sub-folder know that they may need to update based on a new list of folder models.
        /// </summary>
        /// <param name="folders">The new list of folder models.</param>
        public void Update(IReadOnlyList<FolderModel> folders)
        {
            this.FolderItems.Update(folders);
        }
    }
}
