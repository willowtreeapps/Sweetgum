// <copyright file="SerializableRequestModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using RealGoodApps.Companion.Attributes;
using WillowTree.Sweetgum.Client.Serializable.Interfaces;

namespace WillowTree.Sweetgum.Client.Requests.Models
{
    /// <summary>
    /// A model for a request.
    /// </summary>
    public sealed class SerializableRequestModel : ISerializableModel<SerializableRequestModel, RequestModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableRequestModel"/> class.
        /// </summary>
        [CompanionType(typeof(SerializableRequestModel))]
        [CompanionType(typeof(RequestModel))]
        public SerializableRequestModel()
        {
        }

        /// <summary>
        /// Gets or sets the request name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the HTTP method.
        /// </summary>
        public HttpMethod? HttpMethod { get; set; }

        /// <summary>
        /// Gets or sets the request URL.
        /// </summary>
        public string? RequestUrl { get; set; }

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        public IList<SerializableRequestHeaderModel>? RequestHeaders { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Gets or sets the request data.
        /// </summary>
        public string? RequestData { get; set; }

        /// <inheritdoc cref="ISerializableModel{TSerializableModel,TModel}"/>
        public RequestModel ToModel()
        {
            return new(
                this.Name ?? string.Empty,
                this.HttpMethod ?? HttpMethod.Get,
                this.RequestUrl ?? string.Empty,
                this.RequestHeaders?.Select(h => h.ToModel()).ToList() ?? new List<RequestHeaderModel>(),
                this.ContentType ?? string.Empty,
                this.RequestData);
        }
    }
}
