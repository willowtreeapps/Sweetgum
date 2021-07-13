// <copyright file="RequestBuilderWindow.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Linq;
using System.Reactive.Disposables;
using Autofac;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using RealGoodApps.Companion.Attributes;
using WillowTree.Sweetgum.Client.BaseControls.Views;
using WillowTree.Sweetgum.Client.RequestBuilder.ViewModels;
using WillowTree.Sweetgum.Client.Requests.Models;

namespace WillowTree.Sweetgum.Client.RequestBuilder.Views
{
    /// <summary>
    /// The request builder window view class.
    /// This is the window where you are able to define an HTTP request and send. Results are displayed under the inputs.
    /// </summary>
    public partial class RequestBuilderWindow : BaseWindow<RequestBuilderViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBuilderWindow"/> class.
        /// </summary>
        [CompanionType(typeof(RequestBuilderWindow))]
        public RequestBuilderWindow()
        {
        }

        private StackPanel RequestDataStackPanel => this.FindControl<StackPanel>(nameof(this.RequestDataStackPanel));

        private StackPanel ResponseDetailsStackPanel => this.FindControl<StackPanel>(nameof(this.ResponseDetailsStackPanel));

        private Button AddRequestHeaderButton => this.FindControl<Button>(nameof(this.AddRequestHeaderButton));

        private ItemsRepeater RequestHeadersItemsRepeater => this.FindControl<ItemsRepeater>(nameof(this.RequestHeadersItemsRepeater));

        private TextBox RequestDataTextBox => this.FindControl<TextBox>(nameof(this.RequestDataTextBox));

        private TextBox ResponseContentTextBox => this.FindControl<TextBox>(nameof(this.ResponseContentTextBox));

        private TextBox ResponseHeadersTextBox => this.FindControl<TextBox>(nameof(this.ResponseHeadersTextBox));

        private TextBlock ResponseStatusCodeTextBlock => this.FindControl<TextBlock>(nameof(this.ResponseStatusCodeTextBlock));

        private TextBlock ResponseTimeTextBlock => this.FindControl<TextBlock>(nameof(this.ResponseTimeTextBlock));

        private TextBox RequestUrlTextBox => this.FindControl<TextBox>(nameof(this.RequestUrlTextBox));

        private ComboBox ContentTypeComboBox => this.FindControl<ComboBox>(nameof(this.ContentTypeComboBox));

        private ComboBox HttpMethodComboBox => this.FindControl<ComboBox>(nameof(this.HttpMethodComboBox));

        private Button SubmitRequestButton => this.FindControl<Button>(nameof(this.SubmitRequestButton));

        private Button SaveRequestButton => this.FindControl<Button>(nameof(this.SaveRequestButton));

        private Button LoadRequestButton => this.FindControl<Button>(nameof(this.LoadRequestButton));

        /// <summary>
        /// Create an instance of <see cref="RequestBuilderWindow"/>.
        /// </summary>
        /// <param name="requestModel">An instance of <see cref="RequestModel"/>.</param>
        /// <returns>An instance of <see cref="RequestBuilderWindow"/>.</returns>
        public static RequestBuilderWindow Create(RequestModel requestModel)
        {
            var window = new RequestBuilderWindow
            {
                Width = 800,
                Height = 800,
            };

            window.InitializeWindow(builder =>
            {
                builder.RegisterInstance(requestModel).ExternallyOwned();
            });

            window.WhenActivated(disposables =>
            {
                window.Bind(
                        window.ViewModel,
                        viewModel => viewModel.RequestUrl,
                        view => view.RequestUrlTextBox.Text)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.HttpMethods,
                        view => view.HttpMethodComboBox.Items)
                    .DisposeWith(disposables);

                window.Bind(
                        window.ViewModel,
                        viewModel => viewModel.SelectedHttpMethod,
                        view => view.HttpMethodComboBox.SelectedItem)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.RequestHeaders,
                        view => view.RequestHeadersItemsRepeater.Items)
                    .DisposeWith(disposables);

                window.Bind(
                        window.ViewModel,
                        viewModel => viewModel.SelectedContentType,
                        view => view.ContentTypeComboBox.SelectedItem)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.ContentTypes,
                        view => view.ContentTypeComboBox.Items)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.AddRequestHeaderCommand,
                        view => view.AddRequestHeaderButton)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.ResponseContent,
                        view => view.ResponseContentTextBox.Text)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.ResponseHeaders,
                        view => view.ResponseHeadersTextBox.Text)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.ResponseStatusCode,
                        view => view.ResponseStatusCodeTextBlock.Text)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.ResponseStatusCodeColor,
                        view => view.ResponseStatusCodeTextBlock.Foreground)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.ResponseTime,
                        view => view.ResponseTimeTextBlock.Text)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.ShouldShowRequestDataTextBox,
                        view => view.RequestDataStackPanel.IsVisible)
                    .DisposeWith(disposables);

                window.Bind(
                        window.ViewModel,
                        viewModel => viewModel.RequestData,
                        view => view.RequestDataTextBox.Text)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.SendRequestCommand,
                        view => view.SubmitRequestButton)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.SaveCommand,
                        view => view.SaveRequestButton)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.LoadCommand,
                        view => view.LoadRequestButton)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.ShouldShowResponseDetails,
                        view => view.ResponseDetailsStackPanel.IsVisible)
                    .DisposeWith(disposables);

                window.BindInteraction(
                        window.ViewModel,
                        viewModel => viewModel.LoadSpecifyPathInteraction,
                        async (context) =>
                        {
                            var dialog = new OpenFileDialog
                            {
                                AllowMultiple = false,
                            };

                            var path = await dialog.ShowAsync(window);

                            context.SetOutput(path.FirstOrDefault());
                        })
                    .DisposeWith(disposables);

                window.BindInteraction(
                        window.ViewModel,
                        viewModel => viewModel.SaveSpecifyPathInteraction,
                        async (context) =>
                        {
                            var dialog = new SaveFileDialog();
                            var path = await dialog.ShowAsync(window);

                            context.SetOutput(path);
                        })
                    .DisposeWith(disposables);
            });

            return window;
        }

        /// <inheritdoc cref="BaseWindow{TViewModel}"/>
        protected override void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
