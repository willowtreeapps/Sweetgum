// <copyright file="Program.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using Avalonia;
using Avalonia.ReactiveUI;

namespace WillowTree.Sweetgum.Client
{
    /// <summary>
    /// The main program class, which contains the entry point of the application.
    /// </summary>
    public sealed class Program
    {
        /// <summary>
        /// The entry point of the application executable.
        /// Don't use any Avalonia, third-party APIs or any SynchronizationContext-reliant code before AppMain is called:
        /// things aren't initialized yet and stuff might break.
        /// </summary>
        /// <param name="args">The arguments passed into the application.</param>
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        /// <summary>
        /// Avalonia configuration, don't remove; also used by visual designer.
        /// </summary>
        /// <returns>An instance of <see cref="AppBuilder"/>.</returns>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
