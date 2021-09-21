// <copyright file="RequestWorkbookItem.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
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
                    .CombineLatest(
                        this.WhenAnyValue(view => view.ViewModel!.RequestModel),
                        (_, requestModel) => requestModel)
                    .InvokeCommand(this, view => view.ViewModel!.OpenRequestCommand)
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
