// <copyright file="RequestHeaderModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

namespace WillowTree.Sweetgum.Client.Requests.Models
{
    /// <summary>
    /// A request header model.
    /// </summary>
    public sealed class RequestHeaderModel
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

        /// <summary>
        /// Converts an instance of <see cref="RequestHeaderModel"/> to an instance of <see cref="SerializableRequestHeaderModel"/>.
        /// </summary>
        /// <returns>An instance of <see cref="SerializableRequestHeaderModel"/>.</returns>
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
