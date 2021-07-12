// <copyright file="ProxyOptionItem.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Settings.ViewModels;

namespace WillowTree.Sweetgum.Client.Settings.Views
{
    /// <summary>
    /// An individual proxy option within the combo box.
    /// </summary>
    public partial class ProxyOptionItem : ReactiveUserControl<ProxyOptionViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyOptionItem"/> class.
        /// </summary>
        public ProxyOptionItem()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(
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
