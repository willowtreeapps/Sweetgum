// <copyright file="ErrorDialog.axaml.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WillowTree.Sweetgum.Client.Views
{
    /// <summary>
    /// A class for showing a simple error message.
    /// </summary>
    /// <remarks>This class does not derive from <see cref="WillowTree.Sweetgum.Client.BaseControls.Views.BaseWindow{TViewModel}"/> because
    /// it only displays a simple string and doesn't interact with any model objects.</remarks>
    public partial class ErrorDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDialog"/> class.
        /// </summary>
        /// <param name="exception">The exception whose message to display.</param>
        /// <param name="parent">The window that owns this dialog.</param>
        public ErrorDialog(Exception exception, WindowBase parent)
        {
            this.Exception = exception;
            this.Owner = parent;
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            TextBlock block = this.Find<TextBlock>("txtMessage");
            block.Text = this.Exception.Message;

            this.Find<Button>("btnOk").Click += (s, e) => this.Close();

            // We want to constrain the dialog to the message as closely as possible,
            // but still be within our parent, as well as have a reasonable minimum size.
            // Width should be 400 <= error message with a 16-pixel margin <= parent's width.
            // Height should be just enough for the text, button, and margin.
            block.Measure(new Size(Math.Max(400, parent.Width - 32), parent.Height - 62)); // Buttons are 30px high using Avalonia's WPF renderer by default.
            Size desiredSize = block.DesiredSize;
            this.Width = Math.Min(Math.Max(400, desiredSize.Width + 32), parent.Width);
            this.Height = desiredSize.Height + 62;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDialog"/> class.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="parent">The window that owns this dialog.</param>
        public ErrorDialog(string message, WindowBase parent)
            : this(new Exception(message), parent)
        {
        }

#pragma warning disable CS1591, CS8618, SA1600, SA1502
        [Obsolete("Only used for Avalonia infrastructure.")]
        public ErrorDialog() { }
#pragma warning restore CS1591, CS8618, SA1600, SA1502

        /// <summary>
        /// Gets the exception for the message shown in this dialog.
        /// </summary>
        public Exception Exception { get; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
