// <copyright file="EnvironmentsWindow.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
    /// The workbook environments window.
    /// </summary>
    public partial class EnvironmentsWindow : BaseWindow<EnvironmentsViewModel>
    {
        private ComboBox ComboBox => this.FindControl<ComboBox>(nameof(this.ComboBox));

        private ItemsRepeater VariablesItemsRepeater => this.FindControl<ItemsRepeater>(nameof(this.VariablesItemsRepeater));

        private StackPanel CurrentEnvironmentStackPanel => this.FindControl<StackPanel>(nameof(this.CurrentEnvironmentStackPanel));

        private Button AddVariableButton => this.FindControl<Button>(nameof(this.AddVariableButton));

        private Button CreateNewEnvironmentButton => this.FindControl<Button>(nameof(this.CreateNewEnvironmentButton));

        private Button SaveButton => this.FindControl<Button>(nameof(this.SaveButton));

        /// <summary>
        /// Constructs an instance of <see cref="EnvironmentsWindow"/>.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        /// <param name="saveCommand">A command to invoke to save the environments.</param>
        /// <returns>An instance of <see cref="EnvironmentsWindow"/>.</returns>
        public static EnvironmentsWindow Create(
            WorkbookModel workbookModel,
            ReactiveCommand<SaveCommandParameter, Unit> saveCommand)
        {
            var window = new EnvironmentsWindow();

            window.InitializeWindow(builder =>
            {
                builder
                    .RegisterInstance(workbookModel)
                    .ExternallyOwned();

                builder
                    .RegisterInstance(saveCommand)
                    .ExternallyOwned();
            });

            window.WhenActivated(disposables =>
            {
                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.Environments,
                        view => view.ComboBox.Items)
                    .DisposeWith(disposables);

                window.Bind(
                        window.ViewModel,
                        viewModel => viewModel.SelectedEnvironment,
                        view => view.ComboBox.SelectedItem)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.SelectedEnvironment!.Variables,
                        view => view.VariablesItemsRepeater.Items)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.HasSelectedEnvironment,
                        view => view.CurrentEnvironmentStackPanel.IsVisible)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.CanSave,
                        view => view.SaveButton.IsEnabled)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.SelectedEnvironment!.AddVariableCommand,
                        view => view.AddVariableButton)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.CreateNewEnvironmentCommand,
                        view => view.CreateNewEnvironmentButton)
                    .DisposeWith(disposables);

                window.SaveButton
                    .Events()
                    .Click
                    .Select(_ => new SaveCommandParameter
                    {
                        EnvironmentModelsChanges = window.ViewModel!.ToModel(),
                    })
                    .InvokeCommand(window, view => view.ViewModel!.SaveCommand)
                    .DisposeWith(disposables);

                window.BindInteraction(
                    window.ViewModel,
                    viewModel => viewModel.NewEnvironmentInteraction,
                    async (context) =>
                    {
                        var dialog = NewEnvironmentDialog.Create(context.Input);

                        var result = await dialog.ShowDialog<string?>(window);

                        context.SetOutput(result);
                    });
            });

            return window;
        }

        /// <inheritdoc cref="BaseWindow{TViewModel}"/>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            this.ViewModel!.SaveState(
                this.Position,
                this.Width,
                this.Height);
        }

        /// <inheritdoc cref="BaseWindow{TViewModel}"/>
        protected override void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
