// <copyright file="SerializableWorkbookModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using RealGoodApps.Companion.Attributes;
using WillowTree.Sweetgum.Client.Folders.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.Models
{
    /// <summary>
    /// A simplified version of <see cref="WorkbookModel"/> that supports nullability for serialization.
    /// You do not want to use this model throughout the application in most cases.
    /// </summary>
    public sealed class SerializableWorkbookModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableWorkbookModel"/> class.
        /// </summary>
        [CompanionType(typeof(SerializableWorkbookModel))]
        [CompanionType(typeof(WorkbookModel))]
        public SerializableWorkbookModel()
        {
        }

        /// <summary>
        /// Gets or sets the name of the serializable workbook model.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the serializable folders of the serializable workbook model.
        /// </summary>
        public IList<SerializableFolderModel>? Folders { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="SerializableWorkbookModel"/> from an instance of <see cref="WorkbookModel"/>.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        /// <returns>An instance of <see cref="SerializableWorkbookModel"/>.</returns>
        public static SerializableWorkbookModel FromModel(WorkbookModel workbookModel)
        {
            return new SerializableWorkbookModel
            {
                Name = workbookModel.Name,
            };
        }

        /// <summary>
        /// Converts an instance of <see cref="SerializableWorkbookModel"/> to an instance of <see cref="WorkbookModel"/>.
        /// </summary>
        /// <param name="path">The path of the workbook.</param>
        /// <returns>An instance of <see cref="SerializableWorkbookModel"/>.</returns>
        public WorkbookModel ToModel(string path)
        {
            return new(
                this.Name ?? string.Empty,
                path,
                this.Folders?.Select(f => f.ToModel()).ToList() ?? new List<FolderModel>());
        }
    }
}
