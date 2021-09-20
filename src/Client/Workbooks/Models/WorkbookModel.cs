// <copyright file="WorkbookModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using RealGoodApps.Companion.Attributes;
using WillowTree.Sweetgum.Client.Environments.Models;
using WillowTree.Sweetgum.Client.Folders.Models;
using WillowTree.Sweetgum.Client.Requests.Models;
using WillowTree.Sweetgum.Client.Workbooks.Services;
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
        /// <param name="environments">The workbook's environments.</param>
        public WorkbookModel(
            string name,
            string path,
            IReadOnlyList<FolderModel> folders,
            IReadOnlyList<EnvironmentModel> environments)
            : this(name, environments)
        {
            this.Path = path;
            this.Folders = folders;
        }

        [JsonConstructor]
        private WorkbookModel(
            string? name,
            IReadOnlyList<EnvironmentModel>? environments)
        {
            this.Name = name ?? string.Empty;
            this.Path = string.Empty;
            this.Folders = new List<FolderModel>();
            this.Environments = environments ?? new List<EnvironmentModel>();
        }

        private WorkbookModel(WorkbookModel source)
        {
            this.Name = source.Name;
            this.Path = source.Path;
            this.Folders = source.Folders;
            this.Environments = source.Environments;
        }

        /// <summary>
        /// Gets the name of the workbook.
        /// </summary>
        public string Name { get; private init; }

        /// <summary>
        /// Gets the path of the workbook.
        /// </summary>
        [JsonIgnore]
        public string Path { get; private set; }

        /// <summary>
        /// Gets the list of folders in the workbook.
        /// </summary>
        [JsonIgnore]
        public IReadOnlyList<FolderModel> Folders { get; private init; }

        /// <summary>
        /// Gets the list of environments in the workbook.
        /// </summary>
        public IReadOnlyList<EnvironmentModel> Environments { get; private init; }

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
        public WorkbookModel RenameFolder(PathModel path, string newFolderName)
        {
            if (path.IsRoot())
            {
                throw new Exception("You are unable to rename the root folder.");
            }

            var existingFolder = this.GetFolderInternal(path);

            if (existingFolder == null)
            {
                throw new Exception($"The folder path does not exist: {path}");
            }

            if (path.Segments[^1] == newFolderName)
            {
                // Not sure if it makes sense to copy here or just return this.
                return this;
            }

            var newPath = path.GetParent().AddSegment(newFolderName);

            if (this.RequestExists(newPath))
            {
                throw new Exception($"The new path already exists as a request: {newPath}");
            }

            if (this.FolderExists(newPath))
            {
                throw new Exception($"The new path already exists as a folder: {newPath}");
            }

            return new WorkbookModel(this)
            {
                Folders = GetAllFoldersWithNewOrMovedFolder(
                    GetAllFoldersExceptPath(path, this.Folders),
                    PathModel.Root,
                    existingFolder.RewriteParentPath(path.GetParent())),
            };
        }

        /// <summary>
        /// Checks if a folder exists.
        /// </summary>
        /// <param name="path">The path of the folder.</param>
        /// <returns>A value indicating whether or not the folder exists.</returns>
        [CompanionType(typeof(WorkbookViewModel))]
        [CompanionType(typeof(WorkbookNewFolderViewModel))]
        [CompanionType(typeof(WorkbookManager))]
        public bool FolderExists(PathModel path)
        {
            // The root folder is implicit and always exists.
            if (path.IsRoot())
            {
                return true;
            }

            return this.GetFolderInternal(path) != null;
        }

        /// <summary>
        /// Checks if a request exists.
        /// </summary>
        /// <param name="path">The path of the request.</param>
        /// <returns>A value indicating whether or not the request exists.</returns>
        [CompanionType(typeof(WorkbookViewModel))]
        [CompanionType(typeof(WorkbookNewFolderViewModel))]
        public bool RequestExists(PathModel path)
        {
            if (path.IsRoot())
            {
                return false;
            }

            if (path.Segments.Count < 2)
            {
                return false;
            }

            var folderModel = this.GetFolderInternal(path.GetParent());

            if (folderModel == null)
            {
                return false;
            }

            var requestNameSegment = path.Segments[^1];
            return folderModel.Requests.Any(r => r.Name == requestNameSegment);
        }

        /// <summary>
        /// Gets a folder by the full path.
        /// </summary>
        /// <param name="path">The full path of the folder.</param>
        /// <returns>An instance of <see cref="FolderModel"/>.</returns>
        /// <exception cref="Exception">If the folder is not found.</exception>
        [CompanionType(typeof(WorkbookViewModel))]
        public FolderModel GetFolder(PathModel path)
        {
            if (path.IsRoot())
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
        [CompanionType(typeof(WorkbookManager))]
        public WorkbookModel NewFolder(PathModel path)
        {
            if (path.IsRoot())
            {
                throw new Exception("The root folder is implicit and can not be created.");
            }

            var parentPath = path.GetParent();

            if (!this.FolderExists(path.GetParent()))
            {
                throw new Exception($"The parent folder does not exist: {parentPath}");
            }

            if (this.FolderExists(path))
            {
                throw new Exception($"The folder already exists: {path}");
            }

            if (this.RequestExists(path))
            {
                throw new Exception($"The path specified can not be used as it already exists as a request: {path}");
            }

            return new WorkbookModel(this)
            {
                Folders = GetAllFoldersWithNewOrMovedFolder(
                    this.Folders,
                    PathModel.Root,
                    new FolderModel(
                        path.Segments[^1],
                        parentPath,
                        new List<FolderModel>(),
                        new List<RequestModel>())),
            };
        }

        /// <summary>
        /// Creates a new request at the specified path.
        /// </summary>
        /// <param name="path">The path to the new request.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        public WorkbookModel NewRequest(PathModel path)
        {
            if (path.IsRoot())
            {
                throw new Exception("The path specified is not a valid request path.");
            }

            var parent = path.GetParent();

            if (!this.FolderExists(parent))
            {
                throw new Exception($"The folder path does not exist: {parent}");
            }

            if (parent.IsRoot())
            {
                throw new Exception("Requests must not live in the root folder.");
            }

            if (this.RequestExists(path))
            {
                throw new Exception($"The path specified already exists as a request: {path}");
            }

            if (this.FolderExists(path))
            {
                throw new Exception($"The path specified already exists as a folder: {path}");
            }

            IReadOnlyList<FolderModel> AddRequestInternal(IReadOnlyList<FolderModel> folders)
            {
                var newFolders = new List<FolderModel>();

                foreach (var folder in folders)
                {
                    if (folder.GetPath() != parent)
                    {
                        var subfolders = folder.Folders;

                        newFolders.Add(new FolderModel(folder)
                        {
                            Folders = AddRequestInternal(subfolders),
                        });

                        continue;
                    }

                    newFolders.Add(new FolderModel(folder)
                    {
                        Requests = folder.Requests
                            .Append(new RequestModel(
                                path.Segments[^1],
                                parent,
                                HttpMethod.Get,
                                string.Empty,
                                new List<RequestHeaderModel>(),
                                string.Empty,
                                string.Empty))
                            .ToList(),
                    });
                }

                return newFolders;
            }

            return new WorkbookModel(this)
            {
                Folders = AddRequestInternal(this.Folders),
            };
        }

        /// <summary>
        /// Try to get a request in the workbook by path.
        /// If the request is obtained successfully, then the request model and path model will be output to the output variables.
        /// </summary>
        /// <param name="path">The path of the request.</param>
        /// <param name="requestModel">A variable to output the request model.</param>
        /// <returns>A value indicating whether or not the request exists.</returns>
        public bool TryGetRequest(
            PathModel path,
            out RequestModel? requestModel)
        {
            var folders = new Stack<FolderModel>();

            foreach (var folder in this.Folders)
            {
                folders.Push(folder);
            }

            while (folders.Count > 0)
            {
                var currentFolder = folders.Pop();

                if (currentFolder.Requests.Any(request => request.GetPath() == path))
                {
                    requestModel = currentFolder.Requests.First(request => request.GetPath() == path);
                    return true;
                }

                foreach (var subfolder in currentFolder.Folders)
                {
                    folders.Push(subfolder);
                }
            }

            requestModel = null;
            return false;
        }

        /// <summary>
        /// Updates a request in the workbook.
        /// </summary>
        /// <param name="originalPath">The original path of the request to be updated.</param>
        /// <param name="requestModel">An instance of <see cref="RequestModel"/>.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        public WorkbookModel UpdateRequest(
            PathModel originalPath,
            RequestModel requestModel)
        {
            if (!this.TryGetRequest(originalPath, out _))
            {
                throw new Exception("The request you have specified is not in the workbook and can not be updated.");
            }

            var requestPath = requestModel.GetPath();
            var parentPath = requestPath.GetParent();

            if (parentPath == PathModel.Root)
            {
                throw new Exception("The parent path may not be the root.");
            }

            // Are we trying to move the request? If so, we should probably make sure the parent folder exists and there isn't a path collision.
            if (originalPath != requestPath)
            {
                if (!this.FolderExists(requestPath.GetParent()))
                {
                    throw new Exception($"The parent path {parentPath} is not a folder.");
                }

                if (this.FolderExists(requestPath))
                {
                    throw new Exception($"The path {requestPath} is already in use as a folder.");
                }

                if (this.RequestExists(requestPath))
                {
                    throw new Exception($"The path {requestPath} is already in use as a request.");
                }
            }

            IReadOnlyList<FolderModel> ReplaceRequest(IReadOnlyList<FolderModel> folders)
            {
                var newFolders = new List<FolderModel>();

                foreach (var folder in folders)
                {
                    var newRequests = folder.Requests
                        .Where(r => r.GetPath() != originalPath)
                        .ToList();

                    if (folder.GetPath() == parentPath)
                    {
                        newRequests.Add(requestModel);
                    }

                    var newFolder = new FolderModel(folder)
                    {
                        Requests = newRequests,
                        Folders = ReplaceRequest(folder.Folders),
                    };

                    newFolders.Add(newFolder);
                }

                return newFolders;
            }

            return new WorkbookModel(this)
            {
                Folders = ReplaceRequest(this.Folders),
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
            PathModel path,
            PathModel newParentPath)
        {
            if (path.IsRoot())
            {
                throw new Exception("You can not move the root folder.");
            }

            var existingFolder = this.GetFolderInternal(path);

            if (existingFolder == null)
            {
                throw new Exception($"The folder path does not exist: {path}");
            }

            var parentPath = path.GetParent();

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

            var newPath = newParentPath.AddSegment(path.Segments[^1]);

            if (this.FolderExists(newPath))
            {
                throw new Exception($"A folder already exists at {newPath} and can not be overwritten.");
            }

            if (this.RequestExists(newPath))
            {
                throw new Exception($"A request already exists at {newPath} and can not be overwritten.");
            }

            return new WorkbookModel(this)
            {
                Folders = GetAllFoldersWithNewOrMovedFolder(
                    GetAllFoldersExceptPath(path, this.Folders),
                    PathModel.Root,
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
        public WorkbookModel RemoveFolder(PathModel path)
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
        /// Sets the path of a workbook.
        /// </summary>
        /// <param name="path">The path of the workbook.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        [CompanionType(typeof(WorkbookManager))]
        public WorkbookModel WithPath(string path)
        {
            return new(this)
            {
                Path = path,
            };
        }

        /// <summary>
        /// Sets the folders of a workbook.
        /// </summary>
        /// <param name="folders">The folders of the workbook.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        [CompanionType(typeof(WorkbookManager))]
        public WorkbookModel WithFolders(IReadOnlyList<FolderModel> folders)
        {
            return new(this)
            {
                Folders = folders,
            };
        }

        /// <summary>
        /// Gets the folders in the workbook flattened.
        /// </summary>
        /// <returns>The flattened list of folders.</returns>
        public IReadOnlyList<FolderModel> GetFlatFolders()
        {
            var flatFolders = new List<FolderModel>();

            if (this.Folders.Count == 0)
            {
                return flatFolders;
            }

            foreach (var folder in this.Folders)
            {
                flatFolders.Add(folder);
                flatFolders.AddRange(folder.GetFlatChildFolders());
            }

            return flatFolders;
        }

        /// <summary>
        /// Creates a new environment in the workbook with the specified name.
        /// </summary>
        /// <param name="newEnvironmentName">The name of the new environment.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        /// <exception cref="Exception">If the environment specified already exists.</exception>
        public WorkbookModel NewEnvironment(string newEnvironmentName)
        {
            if (this.EnvironmentExists(newEnvironmentName))
            {
                throw new Exception($"The environment named {newEnvironmentName} already exists.");
            }

            return new WorkbookModel(this)
            {
                Environments = this.Environments
                    .Append(new EnvironmentModel(newEnvironmentName, new List<EnvironmentVariableModel>()))
                    .ToList(),
            };
        }

        /// <summary>
        /// Checks if an environment exists by name.
        /// </summary>
        /// <param name="environmentName">The name of the environment to check for.</param>
        /// <returns>A value indicating whether or not the environment exists.</returns>
        public bool EnvironmentExists(string environmentName)
        {
            return this.GetEnvironmentInternal(environmentName) != null;
        }

        /// <summary>
        /// Gets an environment by name.
        /// </summary>
        /// <param name="environmentName">The name of the environment.</param>
        /// <returns>An instance of <see cref="EnvironmentModel"/>.</returns>
        /// <exception cref="Exception">If the environment specified does not exist.</exception>
        public EnvironmentModel GetEnvironment(string environmentName)
        {
            var environment = this.GetEnvironmentInternal(environmentName);

            if (environment == null)
            {
                throw new Exception($"The environment named {environmentName} does not exist.");
            }

            return environment;
        }

        /// <summary>
        /// Update the environments in a workbook.
        /// </summary>
        /// <param name="environmentModels">The new environments.</param>
        /// <returns>An instance of <see cref="WorkbookModel"/>.</returns>
        public WorkbookModel UpdateEnvironments(IReadOnlyList<EnvironmentModel> environmentModels)
        {
            return new(this)
            {
                Environments = environmentModels,
            };
        }

        private static IReadOnlyList<FolderModel> GetAllFoldersExceptPath(PathModel path, IReadOnlyList<FolderModel> folders)
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
            PathModel currentParentPath,
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

        private FolderModel? GetFolderInternal(PathModel path)
        {
            FolderModel? currentFolder = null;

            foreach (var pathSegment in path.Segments)
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

        private EnvironmentModel? GetEnvironmentInternal(string environmentName)
        {
            return this.Environments.FirstOrDefault(e => e.Name == environmentName);
        }
    }
}
