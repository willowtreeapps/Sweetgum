// <copyright file="Dependencies.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using WillowTree.Sweetgum.Client.RequestBuilder.ViewModels;

namespace WillowTree.Sweetgum.Client.DependencyInjection
{
    /// <summary>
    /// Set up our dependency injection container.
    /// </summary>
    public static class Dependencies
    {
        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        public static IServiceProvider? ServiceProvider { get; set; }

        /// <summary>
        /// Configure the services in our dependency injection container.
        /// At the end of this method, the service provider is built and ready for use.
        /// </summary>
        public static void ConfigureServices()
        {
            var services = new ServiceCollection();

            // For more information, see: https://github.com/reactiveui/splat/tree/main/src/Splat.Microsoft.Extensions.DependencyInjection
            services.UseMicrosoftDependencyResolver();
            var resolver = Locator.CurrentMutable;
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            services.AddTransient<RequestBuilderViewModel>();
            services.AddSingleton<ICustomAttributeProvider>(_ => typeof(Dependencies).Assembly);

            // These are normally called inside the `.UseReactiveUI()` callback, but it executes too late.
            services.AddSingleton<IActivationForViewFetcher>(new AvaloniaActivationForViewFetcher());
            services.AddSingleton<IPropertyBindingHook>(new AutoDataTemplateBindingHook());
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;

            ServiceProvider = services.BuildServiceProvider();

            // For more information, see: https://github.com/reactiveui/splat/tree/main/src/Splat.Microsoft.Extensions.DependencyInjection
            ServiceProvider.UseMicrosoftDependencyResolver();
        }
    }
}
