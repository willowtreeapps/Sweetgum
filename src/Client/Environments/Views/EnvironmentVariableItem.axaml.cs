// <copyright file="EnvironmentVariableItem.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Environments.ViewModels;

namespace WillowTree.Sweetgum.Client.Environments.Views
{
    /// <summary>
    /// An individual environment variable.
    /// </summary>
    public partial class EnvironmentVariableItem : ReactiveUserControl<EnvironmentVariableViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentVariableItem"/> class.
        /// </summary>
        public EnvironmentVariableItem()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.Name,
                        view => view.NameTextBox.Text)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.Value,
                        view => view.ValueTextBox.Text)
                    .DisposeWith(disposables);

                this.BindCommand(
                        this.ViewModel!,
                        viewModel => viewModel.RemoveCommand,
                        view => view.RemoveButton)
                    .DisposeWith(disposables);
            });
        }

        private TextBox NameTextBox => this.FindControl<TextBox>(nameof(this.NameTextBox));

        private TextBox ValueTextBox => this.FindControl<TextBox>(nameof(this.ValueTextBox));

        private Button RemoveButton => this.FindControl<Button>(nameof(this.RemoveButton));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
