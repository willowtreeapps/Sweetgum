// <copyright file="WorkbookNewFolderParentItem.axaml.cs" company="WillowTree, LLC">
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
    /// A workbook new folder dialog parent combobox item.
    /// </summary>
    public partial class WorkbookNewFolderParentItem : ReactiveUserControl<WorkbookNewFolderParentViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookNewFolderParentItem"/> class.
        /// </summary>
        public WorkbookNewFolderParentItem()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.Path,
                        view => view.PathTextBlock.Text)
                    .DisposeWith(disposables);
            });
        }

        private TextBlock PathTextBlock => this.FindControl<TextBlock>(nameof(this.PathTextBlock));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
