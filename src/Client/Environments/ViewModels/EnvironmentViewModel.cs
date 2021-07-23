// <copyright file="EnvironmentViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using Avalonia.Collections;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Environments.Models;

namespace WillowTree.Sweetgum.Client.Environments.ViewModels
{
    /// <summary>
    /// The view model for an environment in the environments window.
    /// </summary>
    public sealed class EnvironmentViewModel : ReactiveObject
    {
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentViewModel"/> class.
        /// </summary>
        /// <param name="environment">An instance of <see cref="EnvironmentModel"/>.</param>
        public EnvironmentViewModel(EnvironmentModel? environment)
        {
            this.name = environment != null
                ? environment.Name
                : "Untitled Environment Name";

            this.Variables = new AvaloniaList<EnvironmentVariableViewModel>();

            var removeSubject = new Subject<EnvironmentVariableViewModel>();

            removeSubject
                .Subscribe(environmentVariableViewModel =>
                {
                    if (!this.Variables.Contains(environmentVariableViewModel))
                    {
                        return;
                    }

                    this.Variables.Remove(environmentVariableViewModel);
                });

            if (environment != null)
            {
                this.Variables.AddRange(environment.Variables
                    .Select(v => new EnvironmentVariableViewModel(v, removeSubject))
                    .ToList());
            }

            this.AddVariableCommand = ReactiveCommand.Create(() => this.Variables.Add(new EnvironmentVariableViewModel(null, removeSubject)));
        }

        /// <summary>
        /// Gets or sets the name of the environment.
        /// </summary>
        public string Name
        {
            get => this.name;
            set => this.RaiseAndSetIfChanged(ref this.name, value);
        }

        /// <summary>
        /// Gets the variables for the environment.
        /// </summary>
        public AvaloniaList<EnvironmentVariableViewModel> Variables { get; }

        /// <summary>
        /// Gets a command to create a variable in the environment.
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddVariableCommand { get; }

        /// <summary>
        /// Converts the view model to a read only list of environment models.
        /// </summary>
        /// <returns>A read only list of environment models.</returns>
        public EnvironmentModel ToModel()
        {
            return new(
                this.Name,
                this.Variables
                    .Select(v => new EnvironmentVariableModel(v.Name, v.Value))
                    .ToList());
        }
    }
}
