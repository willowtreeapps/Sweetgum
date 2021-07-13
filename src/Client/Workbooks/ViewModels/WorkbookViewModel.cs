// <copyright file="WorkbookViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
        private readonly ObservableAsPropertyHelper<bool> isRenamingObservableAsPropertyHelper;
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookViewModel"/> class.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        public WorkbookViewModel(WorkbookModel workbookModel)
        {
            this.name = workbookModel.Name;

            var isRenamingBehaviorSubject = new BehaviorSubject<bool>(false);

            this.isRenamingObservableAsPropertyHelper = isRenamingBehaviorSubject
                .ToProperty(this, viewModel => viewModel.IsRenaming);

            this.RenameCommand = ReactiveCommand.Create(() => isRenamingBehaviorSubject.OnNext(true));
            this.FinishRenameCommand = ReactiveCommand.Create(() =>
            {
                isRenamingBehaviorSubject.OnNext(false);
                workbookModel = workbookModel.Rename(this.Name);
            });

            this.SaveCommand = ReactiveCommand.CreateFromTask(
                async (cancellationToken) =>
                {
                    await WorkbookManager.SaveAsync(workbookModel, cancellationToken);
                    return Unit.Default;
                },
                isRenamingBehaviorSubject.Select(r => !r));

            this.WorkbookItems = new WorkbookItemsViewModel(workbookModel.Folders);
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
        /// Gets a value indicating whether or not the workbook is being renamed.
        /// </summary>
        public bool IsRenaming => this.isRenamingObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets a command to rename the workbook.
        /// </summary>
        public ReactiveCommand<Unit, Unit> RenameCommand { get; }

        /// <summary>
        /// Gets a command to finish renaming the workbook.
        /// </summary>
        public ReactiveCommand<Unit, Unit> FinishRenameCommand { get; }

        /// <summary>
        /// Gets a command to save the workbook.
        /// </summary>
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        /// <summary>
        /// Gets the workbook items.
        /// </summary>
        public WorkbookItemsViewModel WorkbookItems { get; }
    }
}