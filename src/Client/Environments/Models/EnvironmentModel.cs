// <copyright file="EnvironmentModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace WillowTree.Sweetgum.Client.Environments.Models
{
    /// <summary>
    /// An environment inside a workbook that holds variables.
    /// </summary>
    public sealed class EnvironmentModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentModel"/> class.
        /// </summary>
        /// <param name="name">The name of the environment.</param>
        /// <param name="variables">The variables of the environment.</param>
        public EnvironmentModel(
            string? name,
            IReadOnlyList<EnvironmentVariableModel>? variables)
        {
            this.Name = name ?? string.Empty;
            this.Variables = variables ?? new List<EnvironmentVariableModel>();
        }

        /// <summary>
        /// Gets the environment name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the variables in the environment.
        /// </summary>
        public IReadOnlyList<EnvironmentVariableModel> Variables { get; }
    }
}
