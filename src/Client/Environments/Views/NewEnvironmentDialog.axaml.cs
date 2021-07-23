// <copyright file="NewEnvironmentDialog.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Reactive.Disposables;
using Autofac;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using WillowTree.Sweetgum.Client.BaseControls.Views;
using WillowTree.Sweetgum.Client.Environments.ViewModels;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.Environments.Views
{
    /// <summary>
    /// The new environment dialog.
    /// </summary>
    public partial class NewEnvironmentDialog : BaseWindow<NewEnvironmentViewModel>
    {
        private TextBox NameTextBox => this.FindControl<TextBox>(nameof(this.NameTextBox));

        private Button SubmitButton => this.FindControl<Button>(nameof(this.SubmitButton));

        /// <summary>
        /// Constructs an instance of <see cref="NewEnvironmentDialog"/>.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        /// <returns>An instance of <see cref="NewEnvironmentDialog"/>.</returns>
        public static NewEnvironmentDialog Create(WorkbookModel workbookModel)
        {
            var dialog = new NewEnvironmentDialog();

            dialog.InitializeWindow(builder =>
            {
                builder
                    .RegisterInstance(workbookModel)
                    .ExternallyOwned();
            });

            dialog.WhenActivated(disposables =>
            {
                dialog.Bind(
                        dialog.ViewModel,
                        viewModel => viewModel.EnvironmentName,
                        view => view.NameTextBox.Text)
                    .DisposeWith(disposables);

                dialog.BindCommand(
                        dialog.ViewModel!,
                        viewModel => viewModel.SubmitCommand,
                        view => view.SubmitButton)
                    .DisposeWith(disposables);

                dialog
                    .WhenAnyObservable(view => view.ViewModel!.SubmitCommand)
                    .Subscribe(result => dialog.Close(result))
                    .DisposeWith(disposables);
            });

            return dialog;
        }

        /// <inheritdoc cref="BaseWindow{TViewModel}"/>
        protected override void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
