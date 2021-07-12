// <copyright file="RequestBuilderWindow.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Linq;
using System.Reactive.Disposables;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
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

            this.ViewModel = Dependencies.Container.Resolve<RequestBuilderViewModel>();

            this.WhenActivated(disposables =>
            {
                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.RequestUrl,
                        view => view.RequestUrlTextBox.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.HttpMethods,
                        view => view.HttpMethodComboBox.Items)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.SelectedHttpMethod,
                        view => view.HttpMethodComboBox.SelectedItem)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.RequestHeaders,
                        view => view.RequestHeadersItemsRepeater.Items)
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

                this.BindCommand(
                        this.ViewModel!,
                        viewModel => viewModel.AddRequestHeaderCommand,
                        view => view.AddRequestHeaderButton)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.ResponseContent,
                        view => view.ResponseContentTextBox.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.ResponseHeaders,
                        view => view.ResponseHeadersTextBox.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.ResponseStatusCode,
                        view => view.ResponseStatusCodeTextBlock.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.ResponseStatusCodeColor,
                        view => view.ResponseStatusCodeTextBlock.Foreground)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.ResponseTime,
                        view => view.ResponseTimeTextBlock.Text)
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

                this.BindCommand(
                        this.ViewModel!,
                        viewModel => viewModel.SaveCommand,
                        view => view.SaveRequestButton)
                    .DisposeWith(disposables);

                this.BindCommand(
                        this.ViewModel!,
                        viewModel => viewModel.LoadCommand,
                        view => view.LoadRequestButton)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.ShouldShowResponseDetails,
                        view => view.ResponseDetailsStackPanel.IsVisible)
                    .DisposeWith(disposables);

                this.BindInteraction(
                        this.ViewModel,
                        viewModel => viewModel.LoadSpecifyPathInteraction,
                        async (context) =>
                        {
                            var dialog = new OpenFileDialog
                            {
                                AllowMultiple = false,
                            };

                            var path = await dialog.ShowAsync(this);

                            context.SetOutput(path.FirstOrDefault());
                        })
                    .DisposeWith(disposables);

                this.BindInteraction(
                        this.ViewModel,
                        viewModel => viewModel.SaveSpecifyPathInteraction,
                        async (context) =>
                        {
                            var dialog = new SaveFileDialog();
                            var path = await dialog.ShowAsync(this);

                            context.SetOutput(path);
                        })
                    .DisposeWith(disposables);
            });
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

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
