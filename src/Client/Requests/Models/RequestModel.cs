// <copyright file="RequestModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Net.Http;

namespace WillowTree.Sweetgum.Client.Requests.Models
{
    /// <summary>
    /// A model for a request.
    /// </summary>
    public sealed class RequestModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestModel"/> class.
        /// </summary>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="requestHeaders">The request headers.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="requestData">The request data.</param>
        public RequestModel(
            HttpMethod httpMethod,
            string requestUrl,
            IReadOnlyList<RequestHeaderModel> requestHeaders,
            string contentType,
            string? requestData)
        {
            this.HttpMethod = httpMethod;
            this.RequestUrl = requestUrl;
            this.RequestHeaders = requestHeaders;
            this.ContentType = contentType;
            this.RequestData = requestData;
        }

        /// <summary>
        /// Gets the HTTP method.
        /// </summary>
        public HttpMethod HttpMethod { get; init; }

        /// <summary>
        /// Gets the request URL.
        /// </summary>
        public string RequestUrl { get; init; }

        /// <summary>
        /// Gets the request headers.
        /// </summary>
        public IReadOnlyList<RequestHeaderModel> RequestHeaders { get; init; }

        /// <summary>
        /// Gets the content type.
        /// </summary>
        public string ContentType { get; init; }

        /// <summary>
        /// Gets the request data.
        /// </summary>
        public string? RequestData { get; init; }
    }
}
