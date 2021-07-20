// <copyright file="WorkbookWindow.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive.Disposables;
using System.Reactive.Linq;
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
    /// The workbook window.
    /// </summary>
    public partial class WorkbookWindow : BaseWindow<WorkbookViewModel>
    {
        private TextBlock NameTextBlock => this.FindControl<TextBlock>(nameof(this.NameTextBlock));

        private TextBox RenameTextBox => this.FindControl<TextBox>(nameof(this.RenameTextBox));

        private Button RenameButton => this.FindControl<Button>(nameof(this.RenameButton));

        private Button FinishRenameButton => this.FindControl<Button>(nameof(this.FinishRenameButton));

        private Button SaveButton => this.FindControl<Button>(nameof(this.SaveButton));

        private Button NewFolderButton => this.FindControl<Button>(nameof(this.NewFolderButton));

        private Button NewRequestButton => this.FindControl<Button>(nameof(this.NewRequestButton));

        private WorkbookItems WorkbookItems => this.FindControl<WorkbookItems>(nameof(this.WorkbookItems));

        /// <summary>
        /// Constructs an instance of <see cref="WorkbookWindow"/>.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        /// <returns>An instance of <see cref="WorkbookWindow"/>.</returns>
        public static WorkbookWindow Create(WorkbookModel workbookModel)
        {
            var window = new WorkbookWindow();

            window.InitializeWindow(builder =>
            {
                builder
                    .RegisterInstance(workbookModel)
                    .ExternallyOwned();
            });

            window.WhenActivated(disposables =>
            {
                window.Bind(
                        window.ViewModel,
                        viewModel => viewModel.Name,
                        view => view.NameTextBlock.Text)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.IsRenaming,
                        view => view.NameTextBlock.IsVisible,
                        r => !r)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.IsRenaming,
                        view => view.RenameButton.IsVisible,
                        r => !r)
                    .DisposeWith(disposables);

                window.Bind(
                        window.ViewModel,
                        viewModel => viewModel.Name,
                        view => view.RenameTextBox.Text)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.IsRenaming,
                        view => view.RenameTextBox.IsVisible)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.IsRenaming,
                        view => view.FinishRenameButton.IsVisible)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.RenameCommand,
                        view => view.RenameButton)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.FinishRenameCommand,
                        view => view.FinishRenameButton)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.NewFolderCommand,
                        view => view.NewFolderButton)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.NewRequestCommand,
                        view => view.NewRequestButton)
                    .DisposeWith(disposables);

                // TODO: Disable the button when saving.
                window.SaveButton
                    .Events()
                    .Click
                    .Select(_ => new SaveCommandParameter())
                    .InvokeCommand(window, view => view.ViewModel!.SaveCommand)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.SaveCommand,
                        view => view.SaveButton)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.WorkbookItems,
                        view => view.WorkbookItems.ViewModel)
                    .DisposeWith(disposables);

                window.BindInteraction(
                    window.ViewModel,
                    viewModel => viewModel.NewFolderInteraction,
                    async (context) =>
                    {
                        var dialog = WorkbookNewFolderDialog.Create(context.Input);

                        var result = await dialog.ShowDialog<PathModel?>(window);

                        context.SetOutput(result);
                    });

                window.BindInteraction(
                    window.ViewModel,
                    viewModel => viewModel.NewRequestInteraction,
                    async (context) =>
                    {
                        var dialog = WorkbookNewRequestDialog.Create(context.Input);

                        var result = await dialog.ShowDialog<PathModel?>(window);

                        context.SetOutput(result);
                    });
            });

            return window;
        }

        /// <inheritdoc cref="BaseWindow{TViewModel}"/>
        protected override void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
