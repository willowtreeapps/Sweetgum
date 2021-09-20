// <copyright file="RequestModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using RealGoodApps.Companion.Attributes;
using WillowTree.Sweetgum.Client.Workbooks.Models;
using WillowTree.Sweetgum.Client.Workbooks.Services;

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
        /// <param name="parentPath">The parent path of the request.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="requestHeaders">The request headers.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="requestData">The request data.</param>
        public RequestModel(
            string name,
            PathModel parentPath,
            HttpMethod httpMethod,
            string requestUrl,
            IReadOnlyList<RequestHeaderModel> requestHeaders,
            string contentType,
            string requestData)
            : this(httpMethod, requestUrl, requestHeaders, contentType, requestData)
        {
            this.Name = name;
            this.ParentPath = parentPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestModel"/> class.
        /// </summary>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="requestHeaders">The request headers.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="requestData">The request data.</param>
        [JsonConstructor]
        public RequestModel(
            HttpMethod? httpMethod,
            string? requestUrl,
            IReadOnlyList<RequestHeaderModel>? requestHeaders,
            string? contentType,
            string? requestData)
        {
            this.Name = string.Empty;
            this.ParentPath = PathModel.Root;
            this.HttpMethod = httpMethod ?? HttpMethod.Get;
            this.RequestUrl = requestUrl ?? string.Empty;
            this.RequestHeaders = requestHeaders ?? new List<RequestHeaderModel>();
            this.ContentType = contentType ?? string.Empty;
            this.RequestData = requestData;
        }

        private RequestModel(RequestModel source)
        {
            this.Name = source.Name;
            this.ParentPath = source.ParentPath;
            this.HttpMethod = source.HttpMethod;
            this.RequestUrl = source.RequestUrl;
            this.RequestHeaders = source.RequestHeaders;
            this.ContentType = source.ContentType;
            this.RequestData = source.RequestData;
        }

        /// <summary>
        /// Gets the request name.
        /// </summary>
        [JsonIgnore]
        public string Name { get; private set; }

        /// <summary>
        /// Gets the request path.
        /// </summary>
        [JsonIgnore]
        public PathModel ParentPath { get; private set; }

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
        /// Sets the name and parent path of the request.
        /// </summary>
        /// <param name="name">The name of the request.</param>
        /// <param name="parentPath">The parent path of the request.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        [CompanionType(typeof(WorkbookManager))]
        public RequestModel WithNameAndParentPath(
            string name,
            PathModel parentPath)
        {
            return new(this)
            {
                Name = name,
                ParentPath = parentPath,
            };
        }

        /// <summary>
        /// Gets the path of the request.
        /// </summary>
        /// <returns>An instance of <see cref="PathModel"/>.</returns>
        public PathModel GetPath()
        {
            return this.ParentPath.AddSegment(this.Name);
        }
    }
}
