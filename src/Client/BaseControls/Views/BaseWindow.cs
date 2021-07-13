// <copyright file="BaseWindow.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using Autofac;
using Avalonia;
using Avalonia.ReactiveUI;
using WillowTree.Sweetgum.Client.DependencyInjection;

namespace WillowTree.Sweetgum.Client.BaseControls.Views
{
    /// <summary>
    /// The base window, which handles registering our DI scope and initialization.
    /// </summary>
    /// <typeparam name="TViewModel">The type of view model.</typeparam>
    public abstract class BaseWindow<TViewModel> : ReactiveWindow<TViewModel>
        where TViewModel : class
    {
        private ILifetimeScope? lifetimeScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWindow{TViewModel}"/> class.
        /// </summary>
        protected BaseWindow()
        {
            // Until we have a better way to solve the Avalonia XAML-loader issue not being usable in the base window.
            // ReSharper disable once VirtualMemberCallInConstructor
            this.InitializeComponent();

#if DEBUG
            // TODO: Do we need this on every window? If so, is there a way to ensure we can't forget it.
            this.AttachDevTools();
#endif
        }

        /// <summary>
        /// Initialize the window within a factory create method of the super type.
        /// You must call this immediately after constructing the supertype to ensure that your lifetime scope is initialized
        /// and that your view model is resolved.
        /// </summary>
        /// <param name="registerDependencies">An action to register dependencies into the lifetime scope.</param>
        protected void InitializeWindow(Action<ContainerBuilder>? registerDependencies = null)
        {
            this.lifetimeScope = registerDependencies == null
                ? Dependencies.Container.BeginLifetimeScope()
                : Dependencies.Container.BeginLifetimeScope(registerDependencies);

            this.ViewModel = this.lifetimeScope.Resolve<TViewModel>();
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

        /// <summary>
        /// Resolve as service from the window's lifetime scope.
        /// </summary>
        /// <typeparam name="TService">The type of service to resolve.</typeparam>
        /// <returns>An instance of the service.</returns>
        /// <exception cref="InvalidOperationException">If the lifetime scope has not yet been initialized or has been disposed.</exception>
        protected TService Resolve<TService>()
            where TService : notnull
        {
            if (this.lifetimeScope == null)
            {
                throw new InvalidOperationException("The lifetime scope has not yet been initialized or has been disposed.");
            }

            return this.lifetimeScope.Resolve<TService>();
        }

        /// <summary>
        /// Initialize the Avalonia component.
        /// </summary>
        protected abstract void InitializeComponent();
    }
}
