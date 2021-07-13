// <copyright file="WorkbookModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using RealGoodApps.Companion.Attributes;
using WillowTree.Sweetgum.Client.Folders.Models;
using WillowTree.Sweetgum.Client.Requests.Models;
using WillowTree.Sweetgum.Client.Workbooks.ViewModels;

namespace WillowTree.Sweetgum.Client.Workbooks.Models
{
    /// <summary>
    /// A model for a workbook.
    /// </summary>
    public sealed class WorkbookModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookModel"/> class.
        /// </summary>
        /// <param name="name">The workbook's name.</param>
        /// <param name="path">The workbook's path.</param>
        /// <param name="folders">The workbook's folders.</param>
        public WorkbookModel(
            string name,
            string path,
            IReadOnlyList<FolderModel> folders)
        {
            this.Name = name;
            this.Path = path;
            this.Folders = folders;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookModel"/> class.
        /// </summary>
        /// <param name="source">An instance of <see cref="WorkbookModel"/>.</param>
        private WorkbookModel(WorkbookModel source)
        {
            this.Name = source.Name;
            this.Path = source.Path;
            this.Folders = source.Folders;
        }

        /// <summary>
        /// Gets the name of the workbook.
        /// </summary>
        public string Name { get; private init; }

        /// <summary>
        /// Gets the path of the workbook.
        /// </summary>
        public string Path { get; private init; }

        /// <summary>
        /// Gets the list of folders in the workbook.
        /// </summary>
        public IReadOnlyList<FolderModel> Folders { get; private init; }

        /// <summary>
        /// Rename a workbook.
        /// </summary>
        /// <param name="newName">The new name of the workbook.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        [CompanionType(typeof(WorkbookViewModel))]
        public WorkbookModel Rename(string newName)
        {
            return new(this)
            {
                Name = newName,
            };
        }

        /// <summary>
        /// Rename a folder.
        /// </summary>
        /// <param name="path">The path of the folder to rename.</param>
        /// <param name="newFolderName">The new folder name.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        /// <exception cref="Exception">If the folder path does not exist.</exception>
        [CompanionType(typeof(WorkbookViewModel))]
        public WorkbookModel RenameFolder(string path, string newFolderName)
        {
            var existingFolder = this.GetFolder(path);

            if (existingFolder == null)
            {
                throw new Exception($"The folder path does not exist: {path}");
            }

            var (folderName, parentPath) = FolderModel.DecomposePath(path);

            if (folderName == newFolderName)
            {
                // Not sure if it makes sense to copy here or just return this.
                return this;
            }

            return new WorkbookModel(this)
            {
                Folders = GetAllFoldersWithNewOrMovedFolder(
                    GetAllFoldersExceptPath(path, this.Folders),
                    string.Empty,
                    existingFolder.RewriteParentPath(parentPath)),
            };
        }

        /// <summary>
        /// Checks if a folder exists.
        /// </summary>
        /// <param name="path">The path of the folder.</param>
        /// <returns>A value indicating whether or not the folder exists.</returns>
        [CompanionType(typeof(WorkbookViewModel))]
        public bool FolderExists(string path)
        {
            // The root folder is implicit and always exists.
            if (path == string.Empty)
            {
                return true;
            }

            return this.GetFolderInternal(path) != null;
        }

        /// <summary>
        /// Gets a folder by the full path.
        /// </summary>
        /// <param name="path">The full path of the folder.</param>
        /// <returns>An instance of <see cref="FolderModel"/>.</returns>
        /// <exception cref="Exception">If the folder is not found.</exception>
        [CompanionType(typeof(WorkbookViewModel))]
        public FolderModel GetFolder(string path)
        {
            if (path == string.Empty)
            {
                throw new Exception("The root folder is implicit and there is no folder model.");
            }

            var folder = this.GetFolderInternal(path);

            if (folder == null)
            {
                throw new Exception($"The folder specified is not found: {path}");
            }

            return folder;
        }

        /// <summary>
        /// Add a folder to a workbook.
        /// </summary>
        /// <param name="path">The full path of the new folder.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        [CompanionType(typeof(WorkbookViewModel))]
        public WorkbookModel NewFolder(string path)
        {
            var (folderName, parentPath) = FolderModel.DecomposePath(path);

            if (!this.FolderExists(parentPath))
            {
                throw new Exception($"The parent folder does not exist: {parentPath}");
            }

            if (this.FolderExists(path))
            {
                throw new Exception($"The folder already exists: {parentPath}");
            }

            return new WorkbookModel(this)
            {
                Folders = GetAllFoldersWithNewOrMovedFolder(
                    this.Folders,
                    string.Empty,
                    new FolderModel(
                        folderName,
                        parentPath,
                        new List<FolderModel>(),
                        new List<RequestModel>())),
            };
        }

        /// <summary>
        /// Move a folder to another folder in a workbook.
        /// </summary>
        /// <param name="path">The full path of the existing folder.</param>
        /// <param name="newParentPath">The new parent path of the folder.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        [CompanionType(typeof(WorkbookViewModel))]
        public WorkbookModel MoveFolder(
            string path,
            string newParentPath)
        {
            var existingFolder = this.GetFolder(path);

            if (existingFolder == null)
            {
                throw new Exception($"The folder path does not exist: {path}");
            }

            var (_, parentPath) = FolderModel.DecomposePath(path);

            if (newParentPath == parentPath)
            {
                // Not sure if it makes sense to copy here or just return this.
                return this;
            }

            if (!this.FolderExists(newParentPath))
            {
                throw new Exception($"The new parent path folder does not exist: {newParentPath}");
            }

            if (newParentPath.StartsWith(parentPath))
            {
                throw new Exception("Unable to move folder into sub-folder of itself.");
            }

            return new WorkbookModel(this)
            {
                Folders = GetAllFoldersWithNewOrMovedFolder(
                    GetAllFoldersExceptPath(path, this.Folders),
                    string.Empty,
                    existingFolder.RewriteParentPath(newParentPath)),
            };
        }

        /// <summary>
        /// Remove a folder by path.
        /// </summary>
        /// <param name="path">The path of the folder to remove.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        /// <exception cref="Exception">If the folder specified does not exist.</exception>
        [CompanionType(typeof(WorkbookViewModel))]
        public WorkbookModel RemoveFolder(string path)
        {
            if (!this.FolderExists(path))
            {
                throw new Exception($"The folder does not exist: {path}");
            }

            return new WorkbookModel(this)
            {
                Folders = GetAllFoldersExceptPath(path, this.Folders),
            };
        }

        /// <summary>
        /// Converts an instance of <see cref="WorkbookModel"/> to an instance of <see cref="SerializableWorkbookModel"/>.
        /// </summary>
        /// <returns>An instance of <see cref="SerializableWorkbookModel"/>.</returns>
        public SerializableWorkbookModel ToSerializable()
        {
            return new()
            {
                Name = this.Name,
                Folders = this.Folders.Select(f => f.ToSerializable()).ToList(),
            };
        }

        private static IReadOnlyList<FolderModel> GetAllFoldersExceptPath(string path, IReadOnlyList<FolderModel> folders)
        {
            return folders
                .Where(folder => folder.GetPath() != path)
                .Select(folder => new FolderModel(folder)
                {
                    Folders = GetAllFoldersExceptPath(path, folder.Folders),
                })
                .ToList();
        }

        private static IReadOnlyList<FolderModel> GetAllFoldersWithNewOrMovedFolder(
            IReadOnlyList<FolderModel> folders,
            string currentParentPath,
            FolderModel newOrMovedFolder)
        {
            if (newOrMovedFolder.ParentPath == currentParentPath)
            {
                return folders
                    .Append(newOrMovedFolder)
                    .ToList();
            }

            return folders
                .Select(folder => new FolderModel(folder)
                {
                    Folders = GetAllFoldersWithNewOrMovedFolder(
                        folder.Folders,
                        folder.GetPath(),
                        newOrMovedFolder),
                })
                .ToList();
        }

        private FolderModel? GetFolderInternal(string path)
        {
            var pathSegments = path.Split('/');
            FolderModel? currentFolder = null;

            foreach (var pathSegment in pathSegments)
            {
                var folders = currentFolder == null ? this.Folders : currentFolder.Folders;
                currentFolder = folders.FirstOrDefault(f => f.Name == pathSegment);

                if (currentFolder == null)
                {
                    return null;
                }
            }

            return currentFolder;
        }
    }
}
