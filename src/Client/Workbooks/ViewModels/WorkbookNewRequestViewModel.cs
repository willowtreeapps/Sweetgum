// <copyright file="WorkbookNewRequestViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Linq;
using System.Reactive;
using Avalonia.Collections;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.ViewModels
{
    /// <summary>
    /// A workbook new request dialog view model.
    /// </summary>
    public sealed class WorkbookNewRequestViewModel : ReactiveObject
    {
        private WorkbookNewFolderParentViewModel selectedParent;
        private string requestName;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookNewRequestViewModel"/> class.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        public WorkbookNewRequestViewModel(WorkbookModel workbookModel)
        {
            this.requestName = string.Empty;
            this.ParentItems = new AvaloniaList<WorkbookNewFolderParentViewModel>();

            this.ParentItems.AddRange(workbookModel.GetFlatFolders()
                .Select(f => new WorkbookNewFolderParentViewModel(f))
                .ToList());

            // By the time we get here, there will be at least a single parent item.
            this.selectedParent = this.ParentItems.First();

            this.SubmitCommand = ReactiveCommand.Create(() => this.selectedParent.Path.AddSegment(this.RequestName));
        }

        /// <summary>
        /// Gets the list of parent items to display in the combo box.
        /// </summary>
        public AvaloniaList<WorkbookNewFolderParentViewModel> ParentItems { get; }

        /// <summary>
        /// Gets or sets the request name.
        /// </summary>
        public string RequestName
        {
            get => this.requestName;
            set => this.RaiseAndSetIfChanged(ref this.requestName, value);
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
