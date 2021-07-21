// <copyright file="SettingsWindow.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using Autofac;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using WillowTree.Sweetgum.Client.BaseControls.Views;
using WillowTree.Sweetgum.Client.DependencyInjection;
using WillowTree.Sweetgum.Client.ProgramState.Services;
using WillowTree.Sweetgum.Client.Settings.ViewModels;

namespace WillowTree.Sweetgum.Client.Settings.Views
{
    /// <summary>
    /// The settings window view class.
    /// This is the window where you are able to modify and save your settings.
    /// </summary>
    public partial class SettingsWindow : BaseWindow<SettingsViewModel>
    {
        private ProgramStateManager programStateManager = null!;

        private ComboBox ProxyOptionsComboBox => this.FindControl<ComboBox>(nameof(this.ProxyOptionsComboBox));

        private TextBox ProxyHostTextBox => this.FindControl<TextBox>(nameof(this.ProxyHostTextBox));

        private TextBox ProxyPortTextBox => this.FindControl<TextBox>(nameof(this.ProxyPortTextBox));

        private Button SaveButton => this.FindControl<Button>(nameof(this.SaveButton));

        private Button CancelButton => this.FindControl<Button>(nameof(this.CancelButton));

        private StackPanel ManualProxyStackPanel => this.FindControl<StackPanel>(nameof(this.ManualProxyStackPanel));

        /// <summary>
        /// Creates a new instance of the <see cref="SettingsWindow"/> class.
        /// </summary>
        /// <returns>An instance of <see cref="SettingsWindow"/>.</returns>
        public static SettingsWindow Create()
        {
            var window = new SettingsWindow();

            window.InitializeWindow();

            window.programStateManager = Dependencies.Container.Resolve<ProgramStateManager>();

            window.WhenActivated(disposables =>
            {
                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.ProxyOptions,
                        view => view.ProxyOptionsComboBox.Items)
                    .DisposeWith(disposables);

                window.Bind(
                        window.ViewModel,
                        viewModel => viewModel.ProxyOption,
                        view => view.ProxyOptionsComboBox.SelectedItem)
                    .DisposeWith(disposables);

                window.Bind(
                        window.ViewModel,
                        viewModel => viewModel.ProxyHost,
                        view => view.ProxyHostTextBox.Text)
                    .DisposeWith(disposables);

                window.Bind(
                        window.ViewModel,
                        viewModel => viewModel.ProxyPort,
                        view => view.ProxyPortTextBox.Text)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.ManualProxyIsVisible,
                        view => view.ManualProxyStackPanel.IsVisible)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.SaveCommand,
                        view => view.SaveButton)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.CancelCommand,
                        view => view.CancelButton)
                    .DisposeWith(disposables);

                window.WhenAnyObservable(
                        view => view.ViewModel!.SaveCommand,
                        view => view.ViewModel!.CancelCommand)
                    .Subscribe(_ => window.Close())
                    .DisposeWith(disposables);
            });

            return window;
        }

        /// <inheritdoc cref="BaseWindow{TViewModel}"/>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            this.programStateManager.Save(this.programStateManager.CurrentState.UpdateSettingsWindow(
                this.Position,
                this.Width,
                this.Height));
        }

        /// <inheritdoc cref="BaseWindow{TViewModel}"/>
        protected override void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
