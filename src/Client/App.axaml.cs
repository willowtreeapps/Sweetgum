// <copyright file="App.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using WillowTree.Sweetgum.Client.DependencyInjection;
using WillowTree.Sweetgum.Client.ProgramState.Services;
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
            if (Design.IsDesignMode)
            {
                // The Visual Studio designer doesn't run our entry point, so DI and services haven't been set up.
                // Do this now.
                Dependencies.ConfigureServices();
            }

            var settingsManager = Dependencies.Container.Resolve<SettingsManager>();
            settingsManager.Load();

            var programStateManager = Dependencies.Container.Resolve<ProgramStateManager>();
            programStateManager.Load();

            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = MainWindow.Create();

                var programState = programStateManager.CurrentState;
                var mainWindowPosition = programState.MainWindowPosition;
                var mainWindowWidth = programState.MainWindowWidth;
                var mainWindowHeight = programState.MainWindowHeight;

                if (mainWindowPosition != default)
                {
                    desktop.MainWindow.Position = mainWindowPosition;
                }

                desktop.MainWindow.Width = mainWindowWidth > 1
                    ? mainWindowWidth
                    : 800;

                desktop.MainWindow.Height = mainWindowHeight > 1
                    ? mainWindowHeight
                    : 800;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
