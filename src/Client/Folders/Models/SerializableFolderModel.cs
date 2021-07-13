// <copyright file="SerializableFolderModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using RealGoodApps.Companion.Attributes;
using WillowTree.Sweetgum.Client.Requests.Models;
using WillowTree.Sweetgum.Client.Serializable.Interfaces;

namespace WillowTree.Sweetgum.Client.Folders.Models
{
    /// <summary>
    /// A simplified version of <see cref="FolderModel"/> that supports nullability for serialization.
    /// You do not want to use this model throughout the application in most cases.
    /// </summary>
    public sealed class SerializableFolderModel : ISerializableModel<SerializableFolderModel, FolderModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableFolderModel"/> class.
        /// </summary>
        [CompanionType(typeof(FolderModel))]
        [CompanionType(typeof(SerializableFolderModel))]
        public SerializableFolderModel()
        {
        }

        /// <summary>
        /// Gets the name of the folder.
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Gets the parent path of the folder.
        /// </summary>
        public string? ParentPath { get; init; }

        /// <summary>
        /// Gets the subfolders within the folder.
        /// </summary>
        public IList<SerializableFolderModel>? Folders { get; init; }

        /// <summary>
        /// Gets the requests within the folder.
        /// </summary>
        public IList<SerializableRequestModel>? Requests { get; init; }

        /// <inheritdoc cref="ISerializableModel{TSerializableModel,TModel}"/>
        public FolderModel ToModel()
        {
            return new(
                this.Name ?? string.Empty,
                this.ParentPath ?? string.Empty,
                this.Folders?.Select(f => f.ToModel()).ToList() ?? new List<FolderModel>(),
                this.Requests?.Select(r => r.ToModel()).ToList() ?? new List<RequestModel>());
        }
    }
}
