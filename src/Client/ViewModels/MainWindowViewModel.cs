// <copyright file="MainWindowViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

namespace WillowTree.Sweetgum.Client.ViewModels
{
    /// <summary>
    /// The main window view model.
    /// </summary>
    public sealed class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets the greeting for the main window.
        /// </summary>
        public string Greeting => "Welcome to Avalonia!";
    }
}
