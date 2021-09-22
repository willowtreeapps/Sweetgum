// <copyright file="WorkbookViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Collections;
using ReactiveUI;
using WillowTree.Sweetgum.Client.ProgramState.Models;
using WillowTree.Sweetgum.Client.ProgramState.Services;
using WillowTree.Sweetgum.Client.RequestBuilder.ViewModels;
using WillowTree.Sweetgum.Client.Requests.Models;
using WillowTree.Sweetgum.Client.Settings.Services;
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
        private readonly ObservableAsPropertyHelper<bool> canSaveObservableAsPropertyHelper;
        private readonly ProgramStateManager programStateManager;
        private readonly string path;
        private string name;
        private WorkbookItemsViewModel workbookItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookViewModel"/> class.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        /// <param name="programStateManager">An instance of <see cref="ProgramStateManager"/>.</param>
        /// <param name="settingsManager">An instance of <see cref="SettingsManager"/>.</param>
        public WorkbookViewModel(
            WorkbookModel workbookModel,
            ProgramStateManager programStateManager,
            SettingsManager settingsManager)
        {
            this.name = workbookModel.Name;
            this.path = workbookModel.Path;
            this.programStateManager = programStateManager;

            var isRenamingBehaviorSubject = new BehaviorSubject<bool>(false);
            var deferredCanCreateItemBehaviorSubject = new BehaviorSubject<bool>(false);

            this.isRenamingObservableAsPropertyHelper = isRenamingBehaviorSubject
                .ToProperty(this, viewModel => viewModel.IsRenaming);

            this.RenameCommand = ReactiveCommand.Create(() => isRenamingBehaviorSubject.OnNext(true));
            this.FinishRenameCommand = ReactiveCommand.Create(() =>
            {
                isRenamingBehaviorSubject.OnNext(false);
                workbookModel = workbookModel.Rename(this.Name);
            });

            this.RequestBuilderViewModels = new AvaloniaList<RequestBuilderViewModel>();

            this.SaveCommand = ReactiveCommand.CreateFromTask<SaveCommandParameter, Unit>(
                async (saveParameter, cancellationToken) =>
                {
                    if (saveParameter.RequestModelChanges != null)
                    {
                        var beforePath = saveParameter.RequestModelChanges.OriginalPath;
                        var newPath = saveParameter.RequestModelChanges.RequestModel.GetPath();

                        var requestBuilderViewModel =
                            this.RequestBuilderViewModels.FirstOrDefault(r => r.OriginalPath == beforePath);

                        workbookModel = workbookModel.UpdateRequest(
                            saveParameter.RequestModelChanges.OriginalPath,
                            saveParameter.RequestModelChanges.RequestModel);

                        if (requestBuilderViewModel != null)
                        {
                            workbookModel.TryGetRequest(newPath, out var updatedRequestModel);

                            if (updatedRequestModel != null)
                            {
                                requestBuilderViewModel.Update(updatedRequestModel);
                            }
                        }
                    }

                    if (saveParameter.EnvironmentModelsChanges != null)
                    {
                        workbookModel = workbookModel.UpdateEnvironments(saveParameter.EnvironmentModelsChanges);
                    }

                    await WorkbookManager.SaveAsync(workbookModel, cancellationToken);

                    this.WorkbookItems.Update(workbookModel);
                    return Unit.Default;
                },
                isRenamingBehaviorSubject.Select(r => !r));

            this.NewFolderInteraction = new Interaction<WorkbookModel, PathModel?>();

            this.NewFolderCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var newFolderPath = await this.NewFolderInteraction.Handle(workbookModel);

                if (newFolderPath != null)
                {
                    workbookModel = workbookModel.NewFolder(newFolderPath, string.Empty);
                    this.WorkbookItems.Update(workbookModel);
                }

                return Unit.Default;
            });

            this.NewRequestInteraction = new Interaction<WorkbookModel, PathModel?>();

            this.NewRequestCommand = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    var newRequestPath = await this.NewRequestInteraction.Handle(workbookModel);

                    if (newRequestPath != null)
                    {
                        workbookModel = workbookModel.NewRequest(newRequestPath);
                        this.WorkbookItems.Update(workbookModel);
                    }

                    return Unit.Default;
                },
                deferredCanCreateItemBehaviorSubject);

            this.OpenEnvironmentsCommand = ReactiveCommand.Create(() => new OpenEnvironmentsResult(
                workbookModel,
                this.SaveCommand));

            this.canSaveObservableAsPropertyHelper = this.SaveCommand.CanExecute
                .ToProperty(this, viewModel => viewModel.CanSave);

            var workbookState = programStateManager.CurrentState.GetWorkbookStateByPath(workbookModel.Path);

            var openRequestCommand = ReactiveCommand.Create<RequestModel, Unit>((requestModel) =>
            {
                if (this.RequestBuilderViewModels.Any(requestBuilderViewModel =>
                    requestBuilderViewModel.OriginalPath == requestModel.GetPath()))
                {
                    return Unit.Default;
                }

                this.RequestBuilderViewModels.Add(new RequestBuilderViewModel(
                    requestModel,
                    settingsManager,
                    this.SaveCommand));

                return Unit.Default;
            });

            this.workbookItems = new WorkbookItemsViewModel(
                workbookModel,
                workbookModel.Folders,
                this.SaveCommand,
                openRequestCommand,
                workbookState);

            this
                .WhenAnyValue(viewModel => viewModel.WorkbookItems.FolderItems.Items.Count)
                .Select(c => c > 0)
                .Subscribe(deferredCanCreateItemBehaviorSubject);
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
        /// Gets a value indicating whether or not the workbook can be saved.
        /// </summary>
        public bool CanSave => this.canSaveObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets a command to rename the workbook.
        /// </summary>
        public ReactiveCommand<Unit, Unit> RenameCommand { get; }

        /// <summary>
        /// Gets a command to finish renaming the workbook.
        /// </summary>
        public ReactiveCommand<Unit, Unit> FinishRenameCommand { get; }

        /// <summary>
        /// Gets a command to create a new folder in the workbook.
        /// </summary>
        public ReactiveCommand<Unit, Unit> NewFolderCommand { get; }

        /// <summary>
        /// Gets an interaction to prompt to the user for creating a new folder in the workbook.
        /// </summary>
        public Interaction<WorkbookModel, PathModel?> NewFolderInteraction { get; }

        /// <summary>
        /// Gets a command to create a new request in the workbook.
        /// </summary>
        public ReactiveCommand<Unit, Unit> NewRequestCommand { get; }

        /// <summary>
        /// Gets an interaction to prompt to the user for creating a new request in the workbook.
        /// </summary>
        public Interaction<WorkbookModel, PathModel?> NewRequestInteraction { get; }

        /// <summary>
        /// Gets a command to save the workbook.
        /// </summary>
        public ReactiveCommand<SaveCommandParameter, Unit> SaveCommand { get; }

        /// <summary>
        /// Gets a command to open the environments manager.
        /// </summary>
        public ReactiveCommand<Unit, OpenEnvironmentsResult> OpenEnvironmentsCommand { get; }

        /// <summary>
        /// Gets or sets the workbook items.
        /// </summary>
        public WorkbookItemsViewModel WorkbookItems
        {
            get => this.workbookItems;
            set => this.RaiseAndSetIfChanged(ref this.workbookItems, value);
        }

        /// <summary>
        /// Gets the list of request builder view models.
        /// </summary>
        public AvaloniaList<RequestBuilderViewModel> RequestBuilderViewModels { get; }

        /// <summary>
        /// Save the workbook state using the program state manager given the position, width, and height of the window.
        /// </summary>
        /// <param name="windowPosition">The window position.</param>
        /// <param name="windowWidth">The window width.</param>
        /// <param name="windowHeight">The window height.</param>
        public void SaveState(
            PixelPoint windowPosition,
            double windowWidth,
            double windowHeight)
        {
            var previousWorkbookState = this.programStateManager.CurrentState.GetWorkbookStateByPath(this.path);

            static IReadOnlyList<ExpandCollapseStateModel> GetExpandCollapseStates(IReadOnlyList<FolderWorkbookItemViewModel> folders)
            {
                var expandCollapseStates = new List<ExpandCollapseStateModel>();

                foreach (var folder in folders)
                {
                    expandCollapseStates.Add(new ExpandCollapseStateModel(folder.Path, folder.IsExpanded));
                    expandCollapseStates.AddRange(GetExpandCollapseStates(folder.FolderItems.Items));
                }

                return expandCollapseStates;
            }

            var newWorkbookState = new WorkbookStateModel(previousWorkbookState)
            {
                ExpandCollapseStates = GetExpandCollapseStates(this.WorkbookItems.FolderItems.Items),
                WindowPosition = windowPosition,
                WindowWidth = windowWidth,
                WindowHeight = windowHeight,
            };

            this.programStateManager.Save(this.programStateManager.CurrentState.UpdateWorkbook(newWorkbookState));
        }
    }
}
