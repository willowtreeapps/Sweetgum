// <copyright file="FolderWorkbookItem.axaml.cs" company="WillowTree, LLC">
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
    /// A folder workbook item user control.
    /// </summary>
    public partial class FolderWorkbookItem : ReactiveUserControl<FolderWorkbookItemViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderWorkbookItem"/> class.
        /// </summary>
        public FolderWorkbookItem()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.Name,
                        view => view.NameTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.FolderItems,
                        view => view.WorkbookFolderItems.ViewModel)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.RequestItems,
                        view => view.WorkbookRequestItems.ViewModel)
                    .DisposeWith(disposables);
            });
        }

        private TextBlock NameTextBlock => this.FindControl<TextBlock>(nameof(this.NameTextBlock));

        private WorkbookFolderItems WorkbookFolderItems => this.FindControl<WorkbookFolderItems>(nameof(this.WorkbookFolderItems));

        private WorkbookRequestItems WorkbookRequestItems => this.FindControl<WorkbookRequestItems>(nameof(this.WorkbookRequestItems));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
