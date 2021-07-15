// <copyright file="RequestModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

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
        /// <param name="name">The request name.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="requestHeaders">The request headers.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="requestData">The request data.</param>
        /// <returns>An instance of <see cref="RequestModel"/>.</returns>
        [JsonConstructor]
        public RequestModel(
            string? name,
            HttpMethod? httpMethod,
            string? requestUrl,
            IReadOnlyList<RequestHeaderModel>? requestHeaders,
            string? contentType,
            string? requestData)
        {
            this.Name = name ?? string.Empty;
            this.HttpMethod = httpMethod ?? HttpMethod.Get;
            this.RequestUrl = requestUrl ?? string.Empty;
            this.RequestHeaders = requestHeaders ?? new List<RequestHeaderModel>();
            this.ContentType = contentType ?? string.Empty;
            this.RequestData = requestData;
        }

        /// <summary>
        /// Gets the request name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the HTTP method.
        /// </summary>
        public HttpMethod HttpMethod { get; }

        /// <summary>
        /// Gets the request URL.
        /// </summary>
        public string RequestUrl { get; }

        /// <summary>
        /// Gets the request headers.
        /// </summary>
        public IReadOnlyList<RequestHeaderModel> RequestHeaders { get; }

        /// <summary>
        /// Gets the content type.
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// Gets the request data.
        /// </summary>
        public string? RequestData { get; }
    }
}
