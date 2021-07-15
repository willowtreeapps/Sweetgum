// <copyright file="RequestHeaderModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using Newtonsoft.Json;

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
        [JsonConstructor]
        public RequestHeaderModel(
            string? name,
            string? value)
        {
            this.Name = name ?? string.Empty;
            this.Value = value ?? string.Empty;
        }

        /// <summary>
        /// Gets the name of the header.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the header.
        /// </summary>
        public string Value { get; }
    }
}
