// <copyright file="WorkbookNewFolderParentItem.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
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
                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.DisplayPath,
                        view => view.PathTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.DepthWidth,
                        view => view.Rectangle.Width)
                    .DisposeWith(disposables);
            });
        }

        private Rectangle Rectangle => this.FindControl<Rectangle>(nameof(this.Rectangle));

        private TextBlock PathTextBlock => this.FindControl<TextBlock>(nameof(this.PathTextBlock));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
