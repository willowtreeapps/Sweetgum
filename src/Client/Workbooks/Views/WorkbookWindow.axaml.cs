// <copyright file="WorkbookWindow.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive.Disposables;
using Autofac;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using WillowTree.Sweetgum.Client.BaseControls.Views;
using WillowTree.Sweetgum.Client.Workbooks.Models;
using WillowTree.Sweetgum.Client.Workbooks.ViewModels;

namespace WillowTree.Sweetgum.Client.Workbooks.Views
{
    /// <summary>
    /// The workbook window.
    /// </summary>
    public partial class WorkbookWindow : BaseWindow<WorkbookViewModel>
    {
        private WorkbookItems WorkbookItems => this.FindControl<WorkbookItems>(nameof(this.WorkbookItems));

        /// <summary>
        /// Constructs an instance of <see cref="WorkbookWindow"/>.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        /// <returns>An instance of <see cref="WorkbookWindow"/>.</returns>
        public static WorkbookWindow Create(WorkbookModel workbookModel)
        {
            var window = new WorkbookWindow();

            window.InitializeWindow(builder =>
            {
                builder
                    .RegisterInstance(workbookModel)
                    .ExternallyOwned();
            });

            window.WhenActivated(disposables =>
            {
                window.OneWayBind(
                        window.ViewModel,
                        viewModel => viewModel.WorkbookItems,
                        view => view.WorkbookItems.ViewModel)
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
