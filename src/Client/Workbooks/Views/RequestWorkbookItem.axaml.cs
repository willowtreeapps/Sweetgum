// <copyright file="RequestWorkbookItem.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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
                        view => view.NameTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.Level,
                        view => view.RowUserControl.Padding,
                        level => new Thickness(10 + (level * 15) + 15, 10, 0, 10))
                    .DisposeWith(disposables);

                this.Events()
                    .PointerReleased
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this, view => view.ViewModel!.OpenRequestCommand)
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
                        var requestState = workbookState.GetRequestStateByPath(result.RequestModel.GetPath());

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

        private UserControl RowUserControl => this.FindControl<UserControl>(nameof(this.RowUserControl));

        private TextBlock NameTextBlock => this.FindControl<TextBlock>(nameof(this.NameTextBlock));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
