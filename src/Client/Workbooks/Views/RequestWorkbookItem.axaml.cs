// <copyright file="RequestWorkbookItem.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive.Disposables;
using Avalonia.Controls;
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
                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.Name,
                        view => view.NameTextBlock.Text)
                    .DisposeWith(disposables);
            });
        }

        private TextBlock NameTextBlock => this.FindControl<TextBlock>(nameof(this.NameTextBlock));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
