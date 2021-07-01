// <copyright file="ViewLocator.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using WillowTree.Sweetgum.Client.ViewModels;

namespace WillowTree.Sweetgum.Client
{
    /// <summary>
    /// The view locator that is able to take a view model object and find the corresponding view class.
    /// </summary>
    /// <inheritdoc cref="IDataTemplate"/>
    public sealed class ViewLocator : IDataTemplate
    {
        /// <summary>
        /// Gets a value indicating whether or not the view locator supports recycling of the data template.
        /// </summary>
        public bool SupportsRecycling => false;

        /// <summary>
        /// Build a control given a data context (which in our case, is a view model).
        /// </summary>
        /// <param name="data">A data context (in our case, a view model).</param>
        /// <returns>An instance of <see cref="IControl"/>.</returns>
        public IControl Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            // TODO: Should we crash if this happens?
            return new TextBlock { Text = "Not Found: " + name };
        }

        /// <summary>
        /// Check if this data template should handle the data context.
        /// We want to handle any data context that is a view model.
        /// </summary>
        /// <param name="data">A data context (which should be a view model).</param>
        /// <returns>A value indicating whether or not the data template should handle this data context.</returns>
        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}
