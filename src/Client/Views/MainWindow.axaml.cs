// <copyright file="MainWindow.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
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
        private const string WorkbookFileExtension = "sg";

        private static readonly List<FileDialogFilter> FileDialogFilters = new()
        {
            new FileDialogFilter
            {
                Name = "Sweetgum Workbook",
                Extensions = new List<string> { WorkbookFileExtension },
            },
        };

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
                        var dialog = new SaveFileDialog
                        {
                            DefaultExtension = WorkbookFileExtension,
                            Filters = FileDialogFilters,
                        };
                        var path = await dialog.ShowAsync(window);

                        context.SetOutput(string.IsNullOrWhiteSpace(path) ? string.Empty : path);
                    });

                window.BindInteraction(
                    window.ViewModel,
                    viewModel => viewModel.LoadWorkbookSpecifyPathInteraction,
                    async context =>
                    {
                        var dialog = new OpenFileDialog
                        {
                            AllowMultiple = false,
                            Filters = FileDialogFilters,
                        };

                        var path = await dialog.ShowAsync(window);

                        context.SetOutput(path.FirstOrDefault() ?? string.Empty);
                    });

                window.WhenAnyObservable(
                        view => view.ViewModel!.NewWorkbookCommand,
                        view => view.ViewModel!.LoadWorkbookCommand)
                    .Subscribe(workbookModel =>
                    {
                        var workbookWindow = WorkbookWindow.Create(workbookModel);
                        workbookWindow.Show();
                    })
                    .DisposeWith(disposables);

                window.WhenAnyObservable(view => view.ViewModel!.OpenSettingsCommand)
                    .Subscribe(_ =>
                    {
                        var settingsWindow = new SettingsWindow
                        {
                            Width = 800,
                            Height = 800,
                        };
                        settingsWindow.Show();
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
