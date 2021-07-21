// <copyright file="Dependencies.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reflection;
using Autofac;
using ReactiveUI;
using Splat.Autofac;
using WillowTree.Sweetgum.Client.ProgramState.Services;
using WillowTree.Sweetgum.Client.RequestBuilder.ViewModels;
using WillowTree.Sweetgum.Client.Settings.Services;
using WillowTree.Sweetgum.Client.Settings.ViewModels;
using WillowTree.Sweetgum.Client.ViewModels;
using WillowTree.Sweetgum.Client.Workbooks.ViewModels;

namespace WillowTree.Sweetgum.Client.DependencyInjection
{
    /// <summary>
    /// Set up our dependency injection container.
    /// </summary>
    public static class Dependencies
    {
        /// <summary>
        /// Gets the Autofac container.
        /// </summary>
        public static IContainer Container { get; private set; } = null!;

        /// <summary>
        /// Configure the services in our dependency injection container.
        /// At the end of this method, the container is built and ready for use.
        /// </summary>
        public static void ConfigureServices()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<RequestBuilderViewModel>().InstancePerDependency();
            builder.RegisterType<MainWindowViewModel>().InstancePerDependency();
            builder.RegisterType<SettingsViewModel>().InstancePerDependency();
            builder.RegisterType<WorkbookViewModel>().InstancePerDependency();
            builder.RegisterType<WorkbookNewFolderViewModel>().InstancePerDependency();
            builder.RegisterType<WorkbookNewRequestViewModel>().InstancePerDependency();
            builder.RegisterType<SettingsManager>().SingleInstance();
            builder.RegisterType<ProgramStateManager>().SingleInstance();
            builder.Register<ICustomAttributeProvider>(_ => typeof(Dependencies).Assembly).SingleInstance();

            // Creates and sets the Autofac resolver as the Locator
            var autofacResolver = builder.UseAutofacDependencyResolver();

            // Register the resolver in Autofac so it can be later resolved
            builder.RegisterInstance(autofacResolver);

            // Initialize ReactiveUI components
            autofacResolver.InitializeReactiveUI();

            Container = builder.Build();
        }
    }
}
