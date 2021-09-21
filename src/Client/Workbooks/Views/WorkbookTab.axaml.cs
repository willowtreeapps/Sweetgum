// <copyright file="WorkbookTab.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using WillowTree.Sweetgum.Client.RequestBuilder.ViewModels;

namespace WillowTree.Sweetgum.Client.Workbooks.Views
{
    /// <summary>
    /// A workbook tab.
    /// </summary>
    public partial class WorkbookTab : ReactiveUserControl<RequestBuilderViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookTab"/> class.
        /// </summary>
        public WorkbookTab()
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
