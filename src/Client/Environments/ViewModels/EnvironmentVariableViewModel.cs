// <copyright file="EnvironmentVariableViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Reactive;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Environments.Models;

namespace WillowTree.Sweetgum.Client.Environments.ViewModels
{
    /// <summary>
    /// The environment variable view model.
    /// </summary>
    public sealed class EnvironmentVariableViewModel : ReactiveObject
    {
        private string name;
        private string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentVariableViewModel"/> class.
        /// </summary>
        /// <param name="environmentVariable">An instance of <see cref="EnvironmentVariableModel"/>.</param>
        /// <param name="removeObserver">An observer to notify when you want to remove the request header.</param>
        public EnvironmentVariableViewModel(
            EnvironmentVariableModel? environmentVariable,
            IObserver<EnvironmentVariableViewModel> removeObserver)
        {
            this.name = string.Empty;
            this.value = string.Empty;

            if (environmentVariable != null)
            {
                this.name = environmentVariable.Name;
                this.value = environmentVariable.Value;
            }

            this.RemoveCommand = ReactiveCommand.Create(() => removeObserver.OnNext(this));
        }

        /// <summary>
        /// Gets or sets the name of the environment variable.
        /// </summary>
        public string Name
        {
            get => this.name;
            set => this.RaiseAndSetIfChanged(ref this.name, value);
        }

        /// <summary>
        /// Gets or sets the value of the environment variable.
        /// </summary>
        public string Value
        {
            get => this.value;
            set => this.RaiseAndSetIfChanged(ref this.value, value);
        }

        /// <summary>
        /// Gets the command to remove the variable.
        /// </summary>
        public ReactiveCommand<Unit, Unit> RemoveCommand { get; }
    }
}
