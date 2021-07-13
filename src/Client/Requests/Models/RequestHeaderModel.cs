// <copyright file="RequestHeaderModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using WillowTree.Sweetgum.Client.Serializable.Interfaces;

namespace WillowTree.Sweetgum.Client.Requests.Models
{
    /// <summary>
    /// A request header model.
    /// </summary>
    public sealed class RequestHeaderModel : IModel<RequestHeaderModel, SerializableRequestHeaderModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestHeaderModel"/> class.
        /// </summary>
        /// <param name="name">The name of the request header.</param>
        /// <param name="value">The value of the request header.</param>
        public RequestHeaderModel(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets the name of the header.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the header.
        /// </summary>
        public string Value { get; }

        /// <inheritdoc cref="IModel{TModel,TSerializableModel}"/>
        public SerializableRequestHeaderModel ToSerializable()
        {
            return new()
            {
                Name = this.Name,
                Value = this.Value,
            };
        }
    }
}
