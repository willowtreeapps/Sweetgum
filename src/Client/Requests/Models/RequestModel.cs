// <copyright file="RequestModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
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
        /// <param name="name">The request name.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="requestHeaders">The request headers.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="requestData">The request data.</param>
        /// <returns>An instance of <see cref="RequestModel"/>.</returns>
        public RequestModel(
            string name,
            HttpMethod httpMethod,
            string requestUrl,
            IReadOnlyList<RequestHeaderModel> requestHeaders,
            string contentType,
            string? requestData)
        {
            this.Name = name;
            this.HttpMethod = httpMethod;
            this.RequestUrl = requestUrl;
            this.RequestHeaders = requestHeaders;
            this.ContentType = contentType;
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

        /// <summary>
        /// Converts an instance of <see cref="RequestModel"/> to an instance of <see cref="SerializableRequestModel"/>.
        /// </summary>
        /// <returns>An instance of <see cref="SerializableRequestModel"/>.</returns>
        public SerializableRequestModel ToSerializable()
        {
            return new()
            {
                Name = this.Name,
                ContentType = this.ContentType,
                HttpMethod = this.HttpMethod,
                RequestData = this.RequestData,
                RequestHeaders = this.RequestHeaders.Select(h => h.ToSerializable()).ToList(),
                RequestUrl = this.RequestUrl,
            };
        }
    }
}
