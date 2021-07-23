// <copyright file="EnvironmentComboBoxItem.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Environments.ViewModels;

namespace WillowTree.Sweetgum.Client.Environments.Views
{
    /// <summary>
    /// An individual environment within the combo box.
    /// </summary>
    public partial class EnvironmentComboBoxItem : ReactiveUserControl<EnvironmentViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentComboBoxItem"/> class.
        /// </summary>
        public EnvironmentComboBoxItem()
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
