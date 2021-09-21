// <copyright file="WorkbookTabContent.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using WillowTree.Sweetgum.Client.RequestBuilder.ViewModels;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.Views
{
    /// <summary>
    /// A workbook tab.
    /// </summary>
    public partial class WorkbookTabContent : ReactiveUserControl<RequestBuilderViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookTabContent"/> class.
        /// </summary>
        public WorkbookTabContent()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.Name,
                        view => view.NameTextBox.Text)
                    .DisposeWith(disposables);

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

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.ContentTypes,
                        view => view.ContentTypeComboBox.Items)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel,
                        viewModel => viewModel.SelectedContentType,
                        view => view.ContentTypeComboBox.SelectedItem)
                    .DisposeWith(disposables);

                this.BindCommand(
                        this.ViewModel!,
                        viewModel => viewModel.AddRequestHeaderCommand,
                        view => view.AddRequestHeaderButton)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.DisplayResponseText,
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

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.CanSave,
                        view => view.SaveRequestButton.IsEnabled)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.ScrollbarHeight,
                        view => view.ScrollViewer.Height)
                    .DisposeWith(disposables);

                this.Bind(
                        this.ViewModel!,
                        viewModel => viewModel.IsPrettyJsonEnabled,
                        view => view.ResponseContentPrettifyButton.IsChecked)
                    .DisposeWith(disposables);

                this.SaveRequestButton
                    .Events()
                    .Click
                    .Select(_ => new SaveCommandParameter
                    {
                        RequestModelChanges = new RequestModelChangeSet(
                            this.ViewModel!.OriginalPath,
                            this.ViewModel!.ToModel()),
                    })
                    .InvokeCommand(this, view => view.ViewModel!.SaveCommand)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        this.ViewModel,
                        viewModel => viewModel.ShouldShowResponseDetails,
                        view => view.ResponseDetailsStackPanel.IsVisible)
                    .DisposeWith(disposables);
            });
        }

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

        private ScrollViewer ScrollViewer => this.FindControl<ScrollViewer>(nameof(this.ScrollViewer));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
