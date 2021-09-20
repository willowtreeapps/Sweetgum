// <copyright file="RequestBuilderWindow.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Autofac;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using WillowTree.Sweetgum.Client.BaseControls.Views;
using WillowTree.Sweetgum.Client.RequestBuilder.ViewModels;
using WillowTree.Sweetgum.Client.Requests.Models;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.RequestBuilder.Views
{
    /// <summary>
    /// The request builder window view class.
    /// This is the window where you are able to define an HTTP request and send. Results are displayed under the inputs.
    /// </summary>
    public partial class RequestBuilderWindow : BaseWindow<RequestBuilderViewModel>
    {
        private TextBox NameTextBox => this.FindControl<TextBox>(nameof(this.NameTextBox));

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

        private ToggleButton ResponseContentPrettifyButton => this.FindControl<ToggleButton>(nameof(this.ResponseContentPrettifyButton));

        /// <summary>
        /// Create an instance of <see cref="RequestBuilderWindow"/>.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        /// <param name="requestModel">An instance of <see cref="RequestModel"/>.</param>
        /// <param name="saveCommand">A command to save the request model.</param>
        /// <returns>An instance of <see cref="RequestBuilderWindow"/>.</returns>
        public static RequestBuilderWindow Create(
            WorkbookModel workbookModel,
            RequestModel requestModel,
            ReactiveCommand<SaveCommandParameter, Unit> saveCommand)
        {
            var window = new RequestBuilderWindow();

            window.InitializeWindow(builder =>
            {
                builder.RegisterInstance(workbookModel).ExternallyOwned();
                builder.RegisterInstance(requestModel).ExternallyOwned();
                builder.RegisterInstance(saveCommand).ExternallyOwned();
            });

            window.WhenActivated(disposables =>
            {
                window.Bind(
                        window.ViewModel,
                        viewModel => viewModel.Name,
                        view => view.NameTextBox.Text)
                    .DisposeWith(disposables);

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

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.ContentTypes,
                        view => view.ContentTypeComboBox.Items)
                    .DisposeWith(disposables);

                window.Bind(
                        window.ViewModel,
                        viewModel => viewModel.SelectedContentType,
                        view => view.ContentTypeComboBox.SelectedItem)
                    .DisposeWith(disposables);

                window.BindCommand(
                        window.ViewModel!,
                        viewModel => viewModel.AddRequestHeaderCommand,
                        view => view.AddRequestHeaderButton)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.DisplayResponseText,
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

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.CanSave,
                        view => view.SaveRequestButton.IsEnabled)
                    .DisposeWith(disposables);

                window.Bind(
                        window.ViewModel!,
                        viewModel => viewModel.IsPrettyJsonEnabled,
                        view => view.ResponseContentPrettifyButton.IsChecked)
                    .DisposeWith(disposables);

                window.SaveRequestButton
                    .Events()
                    .Click
                    .Select(_ => new SaveCommandParameter
                    {
                        RequestModelChanges = new RequestModelChangeSet(
                            window.ViewModel!.OriginalPath,
                            window.ViewModel!.ToModel()),
                    })
                    .InvokeCommand(window, view => view.ViewModel!.SaveCommand)
                    .DisposeWith(disposables);

                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.ShouldShowResponseDetails,
                        view => view.ResponseDetailsStackPanel.IsVisible)
                    .DisposeWith(disposables);
            });

            return window;
        }

        /// <inheritdoc cref="BaseWindow{TViewModel}"/>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            this.ViewModel!.SaveState(
                this.Position,
                this.Width,
                this.Height);
        }

        /// <inheritdoc cref="BaseWindow{TViewModel}"/>
        protected override void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
