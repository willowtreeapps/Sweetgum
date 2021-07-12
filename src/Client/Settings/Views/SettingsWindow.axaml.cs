// <copyright file="SettingsWindow.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Reactive.Disposables;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using WillowTree.Sweetgum.Client.DependencyInjection;
using WillowTree.Sweetgum.Client.Settings.ViewModels;

namespace WillowTree.Sweetgum.Client.Settings.Views
{
    /// <summary>
    /// The settings window view class.
    /// This is the window where you are able to modify and save your settings.
    /// </summary>
    public partial class SettingsWindow : ReactiveWindow<SettingsViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindow"/> class.
        /// </summary>
        public SettingsWindow()
        {
            this.InitializeComponent();
#if DEBUG
            // TODO: Do we need this on every window? If so, is there a way to ensure we can't forget it.
            this.AttachDevTools();
#endif

            this.ViewModel = Dependencies.Container.Resolve<SettingsViewModel>();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.ProxyOptions,
                        view => view.ProxyOptionsComboBox.Items)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.ProxyOption,
                        view => view.ProxyOptionsComboBox.SelectedItem)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.ProxyHost,
                        view => view.ProxyHostTextBox.Text)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.ProxyPort,
                        view => view.ProxyPortTextBox.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.ManualProxyIsVisible,
                        view => view.ManualProxyStackPanel.IsVisible)
                    .DisposeWith(disposables);

                this.BindCommand(
                        this.ViewModel!,
                        viewModel => viewModel.SaveCommand,
                        view => view.SaveButton)
                    .DisposeWith(disposables);

                this.BindCommand(
                        this.ViewModel!,
                        viewModel => viewModel.CancelCommand,
                        view => view.CancelButton)
                    .DisposeWith(disposables);

                this.WhenAnyObservable(
                        view => view.ViewModel!.SaveCommand,
                        view => view.ViewModel!.CancelCommand)
                    .Subscribe(_ => this.Hide())
                    .DisposeWith(disposables);
            });
        }

        private ComboBox ProxyOptionsComboBox => this.FindControl<ComboBox>(nameof(this.ProxyOptionsComboBox));

        private TextBox ProxyHostTextBox => this.FindControl<TextBox>(nameof(this.ProxyHostTextBox));

        private TextBox ProxyPortTextBox => this.FindControl<TextBox>(nameof(this.ProxyPortTextBox));

        private Button SaveButton => this.FindControl<Button>(nameof(this.SaveButton));

        private Button CancelButton => this.FindControl<Button>(nameof(this.CancelButton));

        private StackPanel ManualProxyStackPanel => this.FindControl<StackPanel>(nameof(this.ManualProxyStackPanel));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
