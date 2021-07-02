// <copyright file="RequestBuilderWindow.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using WillowTree.Sweetgum.Client.DependencyInjection;
using WillowTree.Sweetgum.Client.RequestBuilder.ViewModels;

namespace WillowTree.Sweetgum.Client.RequestBuilder.Views
{
    /// <summary>
    /// The request builder window view class.
    /// This is the window where you are able to define an HTTP request and send. Results are displayed under the inputs.
    /// </summary>
    public partial class RequestBuilderWindow : ReactiveWindow<RequestBuilderViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBuilderWindow"/> class.
        /// </summary>
        public RequestBuilderWindow()
        {
            this.InitializeComponent();
#if DEBUG
            // TODO: Do we need this on every window? If so, is there a way to ensure we can't forget it.
            this.AttachDevTools();
#endif

            this.ViewModel = Dependencies.ServiceProvider?.GetRequiredService<RequestBuilderViewModel>();

            this.WhenActivated(disposables =>
            {
                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.RequestUrl,
                        view => view.RequestUrlTextBox.Text)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.SelectedHttpMethod,
                        view => view.HttpMethodComboBox.SelectedItem)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.HttpMethods,
                        view => view.HttpMethodComboBox.Items)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.SelectedContentType,
                        view => view.ContentTypeComboBox.SelectedItem)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.ContentTypes,
                        view => view.ContentTypeComboBox.Items)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.ResponseContent,
                        view => view.ResponseContentTextBox.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.ResponseStatusCode,
                        view => view.ResponseStatusCodeTextBlock.Text,
                        responseCode => responseCode.ToString())
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.ShouldShowRequestDataTextBox,
                        view => view.RequestDataStackPanel.IsVisible)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.RequestData,
                        view => view.RequestDataTextBox.Text)
                    .DisposeWith(disposables);

                this.BindCommand(
                        this.ViewModel!,
                        viewModel => viewModel.SendRequestCommand,
                        view => view.SubmitRequestButton)
                    .DisposeWith(disposables);
            });
        }

        private StackPanel RequestDataStackPanel => this.FindControl<StackPanel>(nameof(this.RequestDataStackPanel));

        private TextBox RequestDataTextBox => this.FindControl<TextBox>(nameof(this.RequestDataTextBox));

        private TextBox ResponseContentTextBox => this.FindControl<TextBox>(nameof(this.ResponseContentTextBox));

        private TextBlock ResponseStatusCodeTextBlock => this.FindControl<TextBlock>(nameof(this.ResponseStatusCodeTextBlock));

        private TextBox RequestUrlTextBox => this.FindControl<TextBox>(nameof(this.RequestUrlTextBox));

        private ComboBox ContentTypeComboBox => this.FindControl<ComboBox>(nameof(this.ContentTypeComboBox));

        private ComboBox HttpMethodComboBox => this.FindControl<ComboBox>(nameof(this.HttpMethodComboBox));

        private Button SubmitRequestButton => this.FindControl<Button>(nameof(this.SubmitRequestButton));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
