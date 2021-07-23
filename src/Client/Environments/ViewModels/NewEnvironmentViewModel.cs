// <copyright file="NewEnvironmentViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.Environments.ViewModels
{
    /// <summary>
    /// A new environment dialog view model.
    /// </summary>
    public sealed class NewEnvironmentViewModel : ReactiveObject
    {
        private string environmentName;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewEnvironmentViewModel"/> class.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        public NewEnvironmentViewModel(WorkbookModel workbookModel)
        {
            this.environmentName = string.Empty;

            var canExecute = this
                .WhenAnyValue(viewModel => viewModel.EnvironmentName)
                .Select(currentEnvironmentName =>
                {
                    if (string.IsNullOrWhiteSpace(currentEnvironmentName))
                    {
                        return false;
                    }

                    return !workbookModel.EnvironmentExists(currentEnvironmentName);
                });

            this.SubmitCommand = ReactiveCommand.Create(
                () => this.EnvironmentName,
                canExecute);
        }

        /// <summary>
        /// Gets or sets the environment name.
        /// </summary>
        public string EnvironmentName
        {
            get => this.environmentName;
            set => this.RaiseAndSetIfChanged(ref this.environmentName, value);
        }

        /// <summary>
        /// Gets the submit command.
        /// </summary>
        public ReactiveCommand<Unit, string> SubmitCommand { get; }
    }
}
