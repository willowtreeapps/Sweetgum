// <copyright file="App.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using WillowTree.Sweetgum.Client.DependencyInjection;
using WillowTree.Sweetgum.Client.Settings.Services;
using WillowTree.Sweetgum.Client.Views;

namespace WillowTree.Sweetgum.Client
{
    /// <summary>
    /// The main application class, responsible for initializing the main window.
    /// </summary>
    /// <inheritdoc cref="Application"/>
    public sealed class App : Application
    {
        /// <summary>
        /// The initialization method.
        /// </summary>
        /// <inheritdoc cref="Application"/>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Called when the framework initialization has been completed.
        /// Here we initialize the main window of the application.
        /// </summary>
        /// <inheritdoc cref="Application"/>
        public override void OnFrameworkInitializationCompleted()
        {
            var settingsInteractor = Dependencies.Container.Resolve<SettingsManager>();
            settingsInteractor.Load();

            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = MainWindow.Create();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
