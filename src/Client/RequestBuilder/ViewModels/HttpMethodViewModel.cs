// <copyright file="HttpMethodViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Net.Http;
using ReactiveUI;

namespace WillowTree.Sweetgum.Client.RequestBuilder.ViewModels
{
    /// <summary>
    /// The main window view model.
    /// </summary>
    public sealed class HttpMethodViewModel : ReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMethodViewModel"/> class.
        /// </summary>
        /// <param name="httpMethod">An instance of <see cref="HttpMethod"/>.</param>
        public HttpMethodViewModel(HttpMethod httpMethod)
        {
            this.HttpMethod = httpMethod;
        }

        /// <summary>
        /// Gets the HTTP method name.
        /// </summary>
        public string Name => this.HttpMethod.Method;

        /// <summary>
        /// Gets the HTTP method.
        /// </summary>
        public HttpMethod HttpMethod { get; }
    }
}
