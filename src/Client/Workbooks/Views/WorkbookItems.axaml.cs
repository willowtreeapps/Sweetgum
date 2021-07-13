// <copyright file="WorkbookItems.axaml.cs" company="WillowTree, LLC">
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
    /// The workbook items.
    /// </summary>
    public partial class WorkbookItems : ReactiveUserControl<WorkbookItemsViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookItems"/> class.
        /// </summary>
        public WorkbookItems()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.FolderItems,
                        view => view.WorkbookFolderItems.ViewModel)
                    .DisposeWith(disposables);
            });
        }

        private WorkbookFolderItems WorkbookFolderItems => this.FindControl<WorkbookFolderItems>(nameof(this.WorkbookFolderItems));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
