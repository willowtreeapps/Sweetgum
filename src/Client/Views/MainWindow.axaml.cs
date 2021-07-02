// <copyright file="MainWindow.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WillowTree.Sweetgum.Client.RequestBuilder.Views;

namespace WillowTree.Sweetgum.Client.Views
{
    /// <summary>
    /// The main window view class.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.RequestBuilderButton.Click += (sender, args) =>
            {
                var requestBuilderWindow = new RequestBuilderWindow
                {
                    Width = 600,
                    Height = 500,
                };
                requestBuilderWindow.Show();
            };
        }

        private Button RequestBuilderButton => this.FindControl<Button>(nameof(this.RequestBuilderButton));

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
