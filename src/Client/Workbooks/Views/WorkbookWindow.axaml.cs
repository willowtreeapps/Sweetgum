// <copyright file="WorkbookWindow.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using Autofac;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RealGoodApps.Companion.Attributes;
using WillowTree.Sweetgum.Client.DependencyInjection;
using WillowTree.Sweetgum.Client.Workbooks.Models;
using WillowTree.Sweetgum.Client.Workbooks.ViewModels;

namespace WillowTree.Sweetgum.Client.Workbooks.Views
{
    /// <summary>
    /// The workbook window.
    /// </summary>
    public partial class WorkbookWindow : ReactiveWindow<WorkbookViewModel>
    {
        private ILifetimeScope? lifetimeScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookWindow"/> class.
        /// </summary>
        [CompanionType(typeof(WorkbookWindow))]
        public WorkbookWindow()
        {
            this.InitializeComponent();
#if DEBUG
            // TODO: Do we need this on every window? If so, is there a way to ensure we can't forget it.
            this.AttachDevTools();
#endif
        }

        /// <summary>
        /// Constructs an instance of <see cref="WorkbookWindow"/>.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        /// <returns>An instance of <see cref="WorkbookWindow"/>.</returns>
        public static WorkbookWindow Create(WorkbookModel workbookModel)
        {
            var window = new WorkbookWindow();
            window.InitializeWindow(workbookModel);
            return window;
        }

        /// <summary>
        /// Clean up the DI scope and dispose of anything.
        /// </summary>
        /// <param name="e">An instance of <see cref="VisualTreeAttachmentEventArgs"/>.</param>
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (this.lifetimeScope == null)
            {
                return;
            }

            this.lifetimeScope.Dispose();
            this.lifetimeScope = null;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void InitializeWindow(WorkbookModel workbookModel)
        {
            this.lifetimeScope = Dependencies.Container.BeginLifetimeScope(builder =>
            {
                // By registering this model as externally owned, we indicate that it should not be disposed automatically.
                builder
                    .RegisterInstance(workbookModel)
                    .ExternallyOwned();
            });

            this.ViewModel = this.lifetimeScope.Resolve<WorkbookViewModel>();

            this.WhenActivated(disposables =>
            {
            });
        }
    }
}
