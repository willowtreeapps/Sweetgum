// <copyright file="FolderModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RealGoodApps.Companion.Attributes;
using WillowTree.Sweetgum.Client.Requests.Models;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.Folders.Models
{
    /// <summary>
    /// A model for a folder within a workbook.
    /// </summary>
    public sealed class FolderModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderModel"/> class.
        /// </summary>
        /// <param name="name">The name of the folder.</param>
        /// <param name="parentPath">The parent path of the folder.</param>
        /// <param name="folders">The folders within the folder.</param>
        /// <param name="requests">The requests within the folder.</param>
        [JsonConstructor]
        public FolderModel(
            string? name,
            string? parentPath,
            IReadOnlyList<FolderModel>? folders,
            IReadOnlyList<RequestModel>? requests)
        {
            this.Name = name ?? string.Empty;
            this.ParentPath = parentPath ?? string.Empty;
            this.Folders = folders ?? new List<FolderModel>();
            this.Requests = requests ?? new List<RequestModel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderModel"/> class.
        /// </summary>
        /// <param name="source">An instance of <see cref="WorkbookModel"/>.</param>
        [CompanionType(typeof(WorkbookModel))]
        public FolderModel(FolderModel source)
            : this(source.Name, source.ParentPath, source.Folders, source.Requests)
        {
        }

        /// <summary>
        /// Gets the name of the folder.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets the parent path of the folder.
        /// </summary>
        public string ParentPath { get; init; }

        /// <summary>
        /// Gets the subfolders within the folder.
        /// </summary>
        public IReadOnlyList<FolderModel> Folders { get; init; }

        /// <summary>
        /// Gets the requests within the folder.
        /// </summary>
        public IReadOnlyList<RequestModel> Requests { get; init; }

        /// <summary>
        /// Build a full path to a folder.
        /// </summary>
        /// <param name="folderName">The name of the folder.</param>
        /// <param name="parentPath">The parent path of the folder.</param>
        /// <returns>The full path of the folder.</returns>
        public static string BuildPath(string folderName, string parentPath)
        {
            if (parentPath == string.Empty)
            {
                return folderName;
            }

            return $"{parentPath}/{folderName}";
        }

        /// <summary>
        /// Decomposes a folder path into the components, which returns the folder name and the parent path.
        /// </summary>
        /// <param name="path">The full path to the folder.</param>
        /// <returns>A tuple with the folder name and the parent path.</returns>
        public static (string FolderName, string ParentPath) DecomposePath(string path)
        {
            var pieces = path.Split('/');

            if (pieces.Length == 1)
            {
                return (path, string.Empty);
            }

            var parentPath = string.Join('/', pieces.Take(pieces.Length - 1));

            return (pieces.Last(), parentPath);
        }

        /// <summary>
        /// Gets the full path of the folder.
        /// </summary>
        /// <returns>The full path of the folder.</returns>
        public string GetPath()
        {
            return BuildPath(this.Name, this.ParentPath);
        }

        /// <summary>
        /// Re-writes a folder with a new parent path, which handles re-writing all sub-folders parent paths.
        /// </summary>
        /// <param name="newParentPath">The new parent path.</param>
        /// <returns>An instance of <see cref="FolderModel"/>.</returns>
        [CompanionType(typeof(WorkbookModel))]
        public FolderModel RewriteParentPath(string newParentPath)
        {
            return new(this)
            {
                ParentPath = newParentPath,
                Folders = this.Folders
                    .Select(f => f.RewriteParentPath($"{newParentPath}/{this.Name}"))
                    .ToList(),
            };
        }

        /// <summary>
        /// Gets a flattened list of child folders to the folder.
        /// </summary>
        /// <returns>A flattened list of the folder's child folders.</returns>
        public IReadOnlyList<FolderModel> GetFlatChildFolders()
        {
            var flatChildFolders = new List<FolderModel>();

            if (this.Folders.Count == 0)
            {
                return flatChildFolders;
            }

            foreach (var childFolder in this.Folders)
            {
                flatChildFolders.Add(childFolder);
                flatChildFolders.AddRange(childFolder.GetFlatChildFolders());
            }

            return flatChildFolders;
        }
    }
}
