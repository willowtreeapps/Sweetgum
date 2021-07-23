// <copyright file="EnvironmentVariableModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

namespace WillowTree.Sweetgum.Client.Environments.Models
{
    /// <summary>
    /// An environment variable inside an environment.
    /// </summary>
    public sealed class EnvironmentVariableModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentVariableModel"/> class.
        /// </summary>
        /// <param name="name">The name of the environment variable.</param>
        /// <param name="value">The value of the environment variable.</param>
        public EnvironmentVariableModel(
            string? name,
            string? value)
        {
            this.Name = name ?? string.Empty;
            this.Value = value ?? string.Empty;
        }

        /// <summary>
        /// Gets the environment variable name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the environment variable value.
        /// </summary>
        public string Value { get; }
    }
}
