// <copyright file="WorkbookFolderItemsViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Avalonia.Collections;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Folders.Models;
using WillowTree.Sweetgum.Client.ProgramState.Models;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.ViewModels
{
    /// <summary>
    /// A workbook folder items view model.
    /// </summary>
    public sealed class WorkbookFolderItemsViewModel : ReactiveObject
    {
        private readonly ReactiveCommand<SaveCommandParameter, Unit> saveCommand;
        private readonly WorkbookStateModel workbookState;
        private readonly WorkbookModel workbookModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookFolderItemsViewModel"/> class.
        /// </summary>
        /// <param name="workbookModel">The model of the workbook holding the folders.</param>
        /// <param name="folders">A read-only list of <see cref="FolderModel"/>.</param>
        /// <param name="saveCommand">A command to invoke to save the request.</param>
        /// <param name="workbookState">An instance of <see cref="WorkbookStateModel"/>.</param>
        public WorkbookFolderItemsViewModel(
            WorkbookModel workbookModel,
            IReadOnlyList<FolderModel> folders,
            ReactiveCommand<SaveCommandParameter, Unit> saveCommand,
            WorkbookStateModel workbookState)
        {
            this.workbookModel = workbookModel;
            this.saveCommand = saveCommand;
            this.workbookState = workbookState;
            this.Items = new AvaloniaList<FolderWorkbookItemViewModel>();

            // TODO: We might need a DI scope here, but this should be fine for now.
            this.Items.AddRange(folders
                .Select(f => new FolderWorkbookItemViewModel(workbookModel, f, saveCommand, workbookState))
                .ToList());
        }

        /// <summary>
        /// Gets the folder items.
        /// </summary>
        public AvaloniaList<FolderWorkbookItemViewModel> Items { get; }

        /// <summary>
        /// Update the view model by looping the folders and only updated what has changed since the initialization.
        /// </summary>
        /// <param name="folders">The updated workbook model's folders.</param>
        public void Update(IReadOnlyList<FolderModel> folders)
        {
            var existingFolders = this.Items
                .ToList();

            var removedFolders = existingFolders
                .Where(existingFolder => folders.All(f => f.GetPath() != existingFolder.Path))
                .ToList();

            var addedFolders = folders
                .Where(folder => existingFolders.All(f => f.Path != folder.GetPath()))
                .Select(folder => new FolderWorkbookItemViewModel(this.workbookModel, folder, this.saveCommand, this.workbookState))
                .ToList();

            this.Items.RemoveAll(removedFolders);
            this.Items.AddRange(addedFolders);

            foreach (var item in this.Items)
            {
                // Update all the subfolders if necessary.
                item.Update(folders.First(f => f.GetPath() == item.Path));
            }
        }
    }
}
