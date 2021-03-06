// <copyright file="MainWindow.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Autofac;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using WillowTree.Sweetgum.Client.BaseControls.Views;
using WillowTree.Sweetgum.Client.DependencyInjection;
using WillowTree.Sweetgum.Client.ProgramState.Services;
using WillowTree.Sweetgum.Client.Settings.Views;
using WillowTree.Sweetgum.Client.ViewModels;
using WillowTree.Sweetgum.Client.Workbooks.Views;

namespace WillowTree.Sweetgum.Client.Views
{
    /// <summary>
    /// The main window view class.
    /// </summary>
    public partial class MainWindow : BaseWindow<MainWindowViewModel>
    {
        private ProgramStateManager programStateManager = null!;

        private Button SettingsButton => this.FindControl<Button>(nameof(this.SettingsButton));

        private Button NewWorkbookButton => this.FindControl<Button>(nameof(this.NewWorkbookButton));

        private Button LoadWorkbookButton => this.FindControl<Button>(nameof(this.LoadWorkbookButton));

        /// <summary>
        /// Creates a new instance of <see cref="MainWindow"/>.
        /// </summary>
        /// <returns>An instance of <see cref="MainWindow"/>.</returns>
        public static MainWindow Create()
        {
            var window = new MainWindow();

            window.InitializeWindow();

            window.programStateManager = Dependencies.Container.Resolve<ProgramStateManager>();

            window.WhenActivated(disposables =>
            {
                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.NewWorkbookCommand,
                        view => view.NewWorkbookButton)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.LoadWorkbookCommand,
                        view => view.LoadWorkbookButton)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.OpenSettingsCommand,
                        view => view.SettingsButton)
                    .DisposeWith(disposables);

                window.BindInteraction(
                    window.ViewModel,
                    viewModel => viewModel.NewWorkbookSpecifyPathInteraction,
                    async context =>
                    {
                        var dialog = new OpenFolderDialog
                        {
                            Title = "Select a folder for workbook",
                        };
                        var path = await dialog.ShowAsync(window);

                        context.SetOutput(path);
                    });

                window.BindInteraction(
                    window.ViewModel,
                    viewModel => viewModel.LoadWorkbookSpecifyPathInteraction,
                    async context =>
                    {
                        var dialog = new OpenFolderDialog
                        {
                            Title = "Select a folder for workbook",
                        };

                        var path = await dialog.ShowAsync(window);

                        context.SetOutput(path);
                    });

                window.WhenAnyObservable(
                        view => view.ViewModel!.NewWorkbookCommand,
                        view => view.ViewModel!.LoadWorkbookCommand)
                    .Subscribe(workbookModel =>
                    {
                        var workbookWindow = WorkbookWindow.Create(workbookModel);
                        var workbookState = window.programStateManager.CurrentState.GetWorkbookStateByPath(workbookModel.Path);

                        workbookWindow.Show();

                        var windowPosition = workbookState.WindowPosition;
                        var windowWidth = workbookState.WindowWidth;
                        var windowHeight = workbookState.WindowHeight;

                        if (windowPosition != default)
                        {
                            workbookWindow.Position = windowPosition;
                        }

                        if (windowWidth > 1)
                        {
                            workbookWindow.Width = windowWidth;
                        }

                        if (windowHeight > 1)
                        {
                            workbookWindow.Height = windowHeight;
                        }
                    })
                    .DisposeWith(disposables);

                window.WhenAnyObservable(view => view.ViewModel!.OpenSettingsCommand)
                    .Subscribe(_ =>
                    {
                        var settingsWindow = SettingsWindow.Create();
                        settingsWindow.Show();

                        var programState = window.programStateManager.CurrentState;

                        var windowPosition = programState.SettingsWindowPosition;
                        var windowWidth = programState.SettingsWindowWidth;
                        var windowHeight = programState.SettingsWindowHeight;

                        if (windowPosition != default)
                        {
                            settingsWindow.Position = windowPosition;
                        }

                        settingsWindow.Width = windowWidth > 1
                            ? windowWidth
                            : 800;

                        settingsWindow.Height = windowHeight > 1
                            ? windowHeight
                            : 800;
                    })
                    .DisposeWith(disposables);

                window.WhenAnyObservable(
                        view => view.ViewModel!.NewWorkbookCommand.ThrownExceptions,
                        view => view.ViewModel!.LoadWorkbookCommand.ThrownExceptions,
                        view => view.ViewModel!.OpenSettingsCommand.ThrownExceptions)
                    .Subscribe(async exception =>
                    {
                        if (exception is not TaskCanceledException)
                        {
                            await new ErrorDialog(exception, window).ShowDialog(window);
                        }
                    })
                    .DisposeWith(disposables);
            });

            return window;
        }

        /// <inheritdoc cref="BaseWindow{TViewModel}"/>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            this.programStateManager.Save(this.programStateManager.CurrentState.UpdateMainWindow(
                this.Position,
                this.Width,
                this.Height));
        }

        /// <inheritdoc cref="BaseWindow{TViewModel}"/>
        protected override void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
