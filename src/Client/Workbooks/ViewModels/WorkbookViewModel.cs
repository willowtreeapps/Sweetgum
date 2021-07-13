// <copyright file="WorkbookViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using ReactiveUI;
using WillowTree.Sweetgum.Client.Workbooks.Models;
using WillowTree.Sweetgum.Client.Workbooks.Services;

namespace WillowTree.Sweetgum.Client.Workbooks.ViewModels
{
    /// <summary>
    /// A workbook view model.
    /// </summary>
    public sealed class WorkbookViewModel : ReactiveObject
    {
        private readonly WorkbookManager workbookManager;
        private readonly WorkbookModel workbookModel;
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookViewModel"/> class.
        /// </summary>
        /// <param name="workbookManager">An instance of <see cref="WorkbookManager"/>.</param>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        public WorkbookViewModel(
            WorkbookManager workbookManager,
            WorkbookModel workbookModel)
        {
            this.name = workbookModel.Name;

            this.workbookManager = workbookManager;
            this.workbookModel = workbookModel;

            this.WorkbookItems = new WorkbookItemsViewModel(workbookModel);
        }

        /// <summary>
        /// Gets or sets the name of the workbook.
        /// </summary>
        public string Name
        {
            get => this.name;
            set => this.RaiseAndSetIfChanged(ref this.name, value);
        }

        /// <summary>
        /// Gets the workbook items.
        /// </summary>
        public WorkbookItemsViewModel WorkbookItems { get; }
    }
}
