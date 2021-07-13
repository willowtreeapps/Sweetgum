// <copyright file="WorkbookRequestItems.axaml.cs" company="WillowTree, LLC">
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
    /// The workbook request items.
    /// </summary>
    public partial class WorkbookRequestItems : ReactiveUserControl<WorkbookRequestItemsViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookRequestItems"/> class.
        /// </summary>
        public WorkbookRequestItems()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.Items,
                        view => view.ItemsRepeater.Items)
                    .DisposeWith(disposables);
            });
        }

        private ItemsRepeater ItemsRepeater => this.FindControl<ItemsRepeater>(nameof(this.ItemsRepeater));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
