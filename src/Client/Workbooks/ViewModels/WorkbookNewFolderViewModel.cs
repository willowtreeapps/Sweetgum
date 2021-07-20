// <copyright file="WorkbookNewFolderViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Collections;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.ViewModels
{
    /// <summary>
    /// A workbook new folder dialog view model.
    /// </summary>
    public sealed class WorkbookNewFolderViewModel : ReactiveObject
    {
        private WorkbookNewFolderParentViewModel selectedParent;
        private string folderName;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookNewFolderViewModel"/> class.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        public WorkbookNewFolderViewModel(WorkbookModel workbookModel)
        {
            this.folderName = string.Empty;

            var rootParentItem = new WorkbookNewFolderParentViewModel();

            this.ParentItems = new AvaloniaList<WorkbookNewFolderParentViewModel>
            {
                rootParentItem,
            };

            this.ParentItems.AddRange(workbookModel.GetFlatFolders()
                .Select(f => new WorkbookNewFolderParentViewModel(f))
                .ToList());

            this.selectedParent = rootParentItem;

            var canExecute = this
                .WhenAnyValue(
                    viewModel => viewModel.FolderName,
                    viewModel => viewModel.SelectedParentItem)
                .Select(current =>
                {
                    var (currentFolderName, currentSelectedParentItem) = current;

                    if (string.IsNullOrWhiteSpace(currentFolderName))
                    {
                        return false;
                    }

                    var newFolderPath = currentSelectedParentItem.Path.AddSegment(currentFolderName);
                    return !workbookModel.FolderExists(newFolderPath);
                });

            this.SubmitCommand = ReactiveCommand.Create(
                () => this.selectedParent.Path.AddSegment(this.FolderName),
                canExecute);
        }

        /// <summary>
        /// Gets the list of parent items to display in the combo box.
        /// </summary>
        public AvaloniaList<WorkbookNewFolderParentViewModel> ParentItems { get; }

        /// <summary>
        /// Gets or sets the folder name.
        /// </summary>
        public string FolderName
        {
            get => this.folderName;
            set => this.RaiseAndSetIfChanged(ref this.folderName, value);
        }

        /// <summary>
        /// Gets or sets the selected parent item.
        /// </summary>
        public WorkbookNewFolderParentViewModel SelectedParentItem
        {
            get => this.selectedParent;
            set => this.RaiseAndSetIfChanged(ref this.selectedParent, value);
        }

        /// <summary>
        /// Gets the submit command.
        /// </summary>
        public ReactiveCommand<Unit, PathModel> SubmitCommand { get; }
    }
}
