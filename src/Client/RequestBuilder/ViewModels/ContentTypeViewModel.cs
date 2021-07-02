// <copyright file="ContentTypeViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using WillowTree.Sweetgum.Client.ViewModels;

namespace WillowTree.Sweetgum.Client.RequestBuilder.ViewModels
{
    /// <summary>
    /// The content type view model.
    /// </summary>
    public sealed class ContentTypeViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTypeViewModel"/> class.
        /// </summary>
        /// <param name="contentType">The content type (aka the MIME type).</param>
        /// <param name="name">The display name of the content type.</param>
        public ContentTypeViewModel(string contentType, string name)
        {
            this.ContentType = contentType;
            this.Name = name;
        }

        /// <summary>
        /// Gets the content type (aka the MIME type).
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// Gets the display name of the content type.
        /// </summary>
        public string Name { get; }
    }
}
