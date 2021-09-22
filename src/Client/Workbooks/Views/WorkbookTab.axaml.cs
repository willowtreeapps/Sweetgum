// <copyright file="WorkbookTab.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Input;
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

                this.CloseButton
                    .Events()
                    .Click
                    .Select(_ => this.ViewModel)
                    .Where(vm => vm != null)
                    .Select(vm => vm.OriginalPath)
                    .InvokeCommand(this, view => view.ViewModel.CloseRequestCommand)
                    .DisposeWith(disposables);
            });
        }

        private TextBlock NameTextBlock => this.FindControl<TextBlock>(nameof(this.NameTextBlock));

        private Button CloseButton => this.FindControl<Button>(nameof(this.CloseButton));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
