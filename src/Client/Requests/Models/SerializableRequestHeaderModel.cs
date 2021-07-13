// <copyright file="SerializableRequestHeaderModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using RealGoodApps.Companion.Attributes;
using WillowTree.Sweetgum.Client.Serializable.Interfaces;

namespace WillowTree.Sweetgum.Client.Requests.Models
{
    /// <summary>
    /// A simplified version of <see cref="RequestHeaderModel"/> that supports nullability for serialization.
    /// You do not want to use this model throughout the application in most cases.
    /// </summary>
    public sealed class SerializableRequestHeaderModel : ISerializableModel<SerializableRequestHeaderModel, RequestHeaderModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableRequestHeaderModel"/> class.
        /// </summary>
        [CompanionType(typeof(SerializableRequestHeaderModel))]
        [CompanionType(typeof(RequestHeaderModel))]
        public SerializableRequestHeaderModel()
        {
        }

        /// <summary>
        /// Gets or sets the name of the header.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the header.
        /// </summary>
        public string? Value { get; set; }

        /// <inheritdoc cref="ISerializableModel{TSerializableModel,TModel}"/>
        public RequestHeaderModel ToModel()
        {
            return new RequestHeaderModel(
                this.Name ?? string.Empty,
                this.Value ?? string.Empty);
        }
    }
}
