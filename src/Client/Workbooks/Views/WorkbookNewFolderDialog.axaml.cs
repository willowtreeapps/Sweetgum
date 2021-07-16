// <copyright file="WorkbookNewFolderDialog.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Reactive.Disposables;
using Autofac;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using WillowTree.Sweetgum.Client.BaseControls.Views;
using WillowTree.Sweetgum.Client.Workbooks.Models;
using WillowTree.Sweetgum.Client.Workbooks.ViewModels;

namespace WillowTree.Sweetgum.Client.Workbooks.Views
{
    /// <summary>
    /// The workbook new folder dialog.
    /// </summary>
    public partial class WorkbookNewFolderDialog : BaseWindow<WorkbookNewFolderViewModel>
    {
        private TextBox NameTextBox => this.FindControl<TextBox>(nameof(this.NameTextBox));

        private Button SubmitButton => this.FindControl<Button>(nameof(this.SubmitButton));

        private ComboBox ParentComboBox => this.FindControl<ComboBox>(nameof(this.ParentComboBox));

        /// <summary>
        /// Constructs an instance of <see cref="WorkbookNewFolderDialog"/>.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        /// <returns>An instance of <see cref="WorkbookNewFolderDialog"/>.</returns>
        public static WorkbookNewFolderDialog Create(WorkbookModel workbookModel)
        {
            var dialog = new WorkbookNewFolderDialog();

            dialog.InitializeWindow(builder =>
            {
                builder
                    .RegisterInstance(workbookModel)
                    .ExternallyOwned();
            });

            dialog.WhenActivated(disposables =>
            {
                dialog.OneWayBind(
                        dialog.ViewModel,
                        viewModel => viewModel.ParentItems,
                        view => view.ParentComboBox.Items)
                    .DisposeWith(disposables);

                dialog.Bind(
                        dialog.ViewModel,
                        viewModel => viewModel.SelectedParentItem,
                        view => view.ParentComboBox.SelectedItem)
                    .DisposeWith(disposables);

                dialog.Bind(
                        dialog.ViewModel,
                        viewModel => viewModel.FolderName,
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
