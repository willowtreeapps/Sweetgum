// <copyright file="MainWindowViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Workbooks.Models;
using WillowTree.Sweetgum.Client.Workbooks.Services;

namespace WillowTree.Sweetgum.Client.ViewModels
{
    /// <summary>
    /// The main window view model.
    /// </summary>
    public sealed class MainWindowViewModel : ReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.NewWorkbookCommand = ReactiveCommand.CreateFromTask<Unit, WorkbookModel>(this.NewWorkbookAsync);
            this.NewWorkbookSpecifyPathInteraction = new Interaction<Unit, string>();

            this.LoadWorkbookCommand = ReactiveCommand.CreateFromTask<Unit, WorkbookModel>(this.LoadWorkbookAsync);
            this.LoadWorkbookSpecifyPathInteraction = new Interaction<Unit, string>();
        }

        /// <summary>
        /// Gets the command to create a new workbook.
        /// </summary>
        public ReactiveCommand<Unit, WorkbookModel> NewWorkbookCommand { get; }

        /// <summary>
        /// Gets the command to load an existing workbook.
        /// </summary>
        public ReactiveCommand<Unit, WorkbookModel> LoadWorkbookCommand { get; }

        /// <summary>
        /// Gets the interaction to specify a path for a new workbook.
        /// </summary>
        public Interaction<Unit, string> NewWorkbookSpecifyPathInteraction { get; }

        /// <summary>
        /// Gets the interaction to specify a path for an existing workbook.
        /// </summary>
        public Interaction<Unit, string> LoadWorkbookSpecifyPathInteraction { get; }

        private async Task<WorkbookModel> NewWorkbookAsync(
            Unit input,
            CancellationToken cancellationToken)
        {
            var path = await this.NewWorkbookSpecifyPathInteraction.Handle(Unit.Default);

            return await WorkbookManager.NewAsync(path, cancellationToken);
        }

        private async Task<WorkbookModel> LoadWorkbookAsync(
            Unit input,
            CancellationToken cancellationToken)
        {
            var path = await this.LoadWorkbookSpecifyPathInteraction.Handle(Unit.Default);

            return await WorkbookManager.LoadAsync(path, cancellationToken);
        }
    }
}
