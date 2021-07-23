// <copyright file="EnvironmentsViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Collections;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Environments.Models;
using WillowTree.Sweetgum.Client.ProgramState.Models;
using WillowTree.Sweetgum.Client.ProgramState.Services;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.Environments.ViewModels
{
    /// <summary>
    /// The view model for the environments window.
    /// </summary>
    public sealed class EnvironmentsViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly ObservableAsPropertyHelper<bool> canSaveObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<bool> hasSelectedEnvironmentObservableAsPropertyHelper;
        private readonly ProgramStateManager programStateManager;
        private readonly string path;
        private EnvironmentViewModel? selectedEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentsViewModel"/> class.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        /// <param name="saveCommand">A command to invoke to save the environments.</param>
        /// <param name="programStateManager">An instance of <see cref="ProgramStateManager"/>.</param>
        public EnvironmentsViewModel(
            WorkbookModel workbookModel,
            ReactiveCommand<SaveCommandParameter, Unit> saveCommand,
            ProgramStateManager programStateManager)
        {
            this.path = workbookModel.Path;
            this.programStateManager = programStateManager;
            this.Activator = new ViewModelActivator();

            this.Environments = new AvaloniaList<EnvironmentViewModel>(workbookModel.Environments
                .Select(e => new EnvironmentViewModel(e))
                .ToList());

            this.selectedEnvironment = this.Environments.FirstOrDefault();

            this.hasSelectedEnvironmentObservableAsPropertyHelper = this
                .WhenAnyValue(viewModel => viewModel.SelectedEnvironment)
                .Select(currentSelectedEnvironment => currentSelectedEnvironment != null)
                .ToProperty(this, viewModel => viewModel.HasSelectedEnvironment);

            this.NewEnvironmentInteraction = new Interaction<WorkbookModel, string?>();

            this.CreateNewEnvironmentCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var newEnvironmentName = await this.NewEnvironmentInteraction.Handle(workbookModel);

                if (newEnvironmentName != null)
                {
                    workbookModel = workbookModel.NewEnvironment(newEnvironmentName);
                    var newEnvironment = new EnvironmentViewModel(workbookModel.GetEnvironment(newEnvironmentName));
                    this.Environments.Add(newEnvironment);
                    this.SelectedEnvironment = newEnvironment;
                }

                return Unit.Default;
            });

            this.SaveCommand = saveCommand;

            this.canSaveObservableAsPropertyHelper = saveCommand.CanExecute
                .ToProperty(this, viewModel => viewModel.CanSave);

            this.WhenActivated(disposables =>
            {
                this.canSaveObservableAsPropertyHelper.DisposeWith(disposables);
            });
        }

        /// <summary>
        /// Gets the view model activator.
        /// </summary>
        public ViewModelActivator Activator { get; }

        /// <summary>
        /// Gets the list of environment view models.
        /// </summary>
        public AvaloniaList<EnvironmentViewModel> Environments { get; }

        /// <summary>
        /// Gets or sets the selected environment.
        /// </summary>
        public EnvironmentViewModel? SelectedEnvironment
        {
            get => this.selectedEnvironment;
            set => this.RaiseAndSetIfChanged(ref this.selectedEnvironment, value);
        }

        /// <summary>
        /// Gets a value indicating whether or not there has been an environment selected.
        /// </summary>
        public bool HasSelectedEnvironment => this.hasSelectedEnvironmentObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets a command to create a new environment.
        /// </summary>
        public ReactiveCommand<Unit, Unit> CreateNewEnvironmentCommand { get; }

        /// <summary>
        /// Gets an interaction that prompts to create a new environment in the workbook.
        /// </summary>
        public Interaction<WorkbookModel, string?> NewEnvironmentInteraction { get; }

        /// <summary>
        /// Gets a command to save the environments to the workbook.
        /// </summary>
        public ReactiveCommand<SaveCommandParameter, Unit> SaveCommand { get; }

        /// <summary>
        /// Gets a value indicating whether or not the environments can be saved.
        /// </summary>
        public bool CanSave => this.canSaveObservableAsPropertyHelper.Value;

        /// <summary>
        /// Converts the view model to a read only list of environment models.
        /// </summary>
        /// <returns>A read only list of environment models.</returns>
        public IReadOnlyList<EnvironmentModel> ToModel()
        {
            return this.Environments.Select(e => e.ToModel()).ToList();
        }

        /// <summary>
        /// Save the environments program state with the window position, width, and height.
        /// </summary>
        /// <param name="position">The position of the window.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        public void SaveState(PixelPoint position, double width, double height)
        {
            var programState = this.programStateManager.CurrentState;
            var workbookState = programState.GetWorkbookStateByPath(this.path);
            var previousEnvironmentsState = workbookState.EnvironmentsState;

            var environmentsState = new EnvironmentsStateModel(previousEnvironmentsState)
            {
                WindowPosition = position,
                WindowWidth = width,
                WindowHeight = height,
            };

            workbookState = workbookState.UpdateEnvironments(environmentsState);
            this.programStateManager.Save(programState.UpdateWorkbook(workbookState));
        }
    }
}
