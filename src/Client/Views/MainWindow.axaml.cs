// <copyright file="MainWindow.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Reactive.Disposables;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using WillowTree.Sweetgum.Client.DependencyInjection;
using WillowTree.Sweetgum.Client.RequestBuilder.Views;
using WillowTree.Sweetgum.Client.Settings.Views;
using WillowTree.Sweetgum.Client.ViewModels;
using WillowTree.Sweetgum.Client.Workbooks.Views;

namespace WillowTree.Sweetgum.Client.Views
{
    /// <summary>
    /// The main window view class.
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.ViewModel = Dependencies.Container.Resolve<MainWindowViewModel>();

            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.RequestBuilderButton.Click += (_, _) =>
            {
                var requestBuilderWindow = new RequestBuilderWindow
                {
                    Width = 800,
                    Height = 800,
                };
                requestBuilderWindow.Show();
            };

            this.SettingsButton.Click += (_, _) =>
            {
                var settingsWindow = new SettingsWindow
                {
                    Width = 800,
                    Height = 800,
                };
                settingsWindow.Show();
            };

            this.WhenActivated(disposables =>
            {
                this.BindCommand(
                        this.ViewModel!,
                        viewModel => viewModel.NewWorkbookCommand,
                        view => view.NewWorkbookButton)
                    .DisposeWith(disposables);

                this.BindCommand(
                        this.ViewModel!,
                        viewModel => viewModel.LoadWorkbookCommand,
                        view => view.LoadWorkbookButton)
                    .DisposeWith(disposables);

                this.BindInteraction(
                    this.ViewModel,
                    viewModel => viewModel.NewWorkbookSpecifyPathInteraction,
                    async context =>
                    {
                        var dialog = new SaveFileDialog();
                        var path = await dialog.ShowAsync(this);

                        context.SetOutput(string.IsNullOrWhiteSpace(path) ? string.Empty : path);
                    });

                this.BindInteraction(
                    this.ViewModel,
                    viewModel => viewModel.LoadWorkbookSpecifyPathInteraction,
                    async context =>
                    {
                        var dialog = new OpenFileDialog
                        {
                            AllowMultiple = false,
                        };

                        var path = await dialog.ShowAsync(this);

                        context.SetOutput(path.FirstOrDefault() ?? string.Empty);
                    });

                this.WhenAnyObservable(
                        view => view.ViewModel!.NewWorkbookCommand,
                        view => view.ViewModel!.LoadWorkbookCommand)
                    .Subscribe(workbookModel =>
                    {
                        var window = WorkbookWindow.Create(workbookModel);
                        window.Show();
                    })
                    .DisposeWith(disposables);
            });
        }

        private Button RequestBuilderButton => this.FindControl<Button>(nameof(this.RequestBuilderButton));

        private Button SettingsButton => this.FindControl<Button>(nameof(this.SettingsButton));

        private Button NewWorkbookButton => this.FindControl<Button>(nameof(this.NewWorkbookButton));

        private Button LoadWorkbookButton => this.FindControl<Button>(nameof(this.LoadWorkbookButton));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
