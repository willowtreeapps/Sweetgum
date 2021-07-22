// <copyright file="RequestWorkbookItem.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Reactive.Disposables;
using Autofac;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using WillowTree.Sweetgum.Client.DependencyInjection;
using WillowTree.Sweetgum.Client.ProgramState.Services;
using WillowTree.Sweetgum.Client.RequestBuilder.Views;
using WillowTree.Sweetgum.Client.Workbooks.ViewModels;

namespace WillowTree.Sweetgum.Client.Workbooks.Views
{
    /// <summary>
    /// A request workbook item user control.
    /// </summary>
    public partial class RequestWorkbookItem : ReactiveUserControl<RequestWorkbookItemViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestWorkbookItem"/> class.
        /// </summary>
        public RequestWorkbookItem()
        {
            this.InitializeComponent();

            var programStateManager = Dependencies.Container.Resolve<ProgramStateManager>();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.Name,
                        view => view.NameButton.Content)
                    .DisposeWith(disposables);

                this.BindCommand(
                        this.ViewModel!,
                        viewModel => viewModel.OpenRequestCommand,
                        view => view.NameButton)
                    .DisposeWith(disposables);

                this
                    .WhenAnyObservable(view => view.ViewModel!.OpenRequestCommand)
                    .Subscribe(result =>
                    {
                        var window = RequestBuilderWindow.Create(
                            result.WorkbookModel,
                            result.RequestModel,
                            result.SaveCommand);
                        window.Show();

                        var workbookState = programStateManager.CurrentState.GetWorkbookStateByPath(result.WorkbookModel.Path);
                        var requestState = workbookState.GetRequestStateById(result.RequestModel.Id);

                        var windowPosition = requestState.WindowPosition;
                        var windowWidth = requestState.WindowWidth;
                        var windowHeight = requestState.WindowHeight;

                        if (windowPosition != default)
                        {
                            window.Position = windowPosition;
                        }

                        window.Width = windowWidth > 1
                            ? windowWidth
                            : 800;

                        window.Height = windowHeight > 1
                            ? windowHeight
                            : 800;
                    })
                    .DisposeWith(disposables);
            });
        }

        private Button NameButton => this.FindControl<Button>(nameof(this.NameButton));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
