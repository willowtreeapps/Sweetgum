// <copyright file="OpenEnvironmentsResult.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive;
using ReactiveUI;

namespace WillowTree.Sweetgum.Client.Workbooks.Models
{
    /// <summary>
    /// The result of the open environments command.
    /// </summary>
    public sealed class OpenEnvironmentsResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenEnvironmentsResult"/> class.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        /// <param name="saveCommand">A command to invoke to save the request.</param>
        public OpenEnvironmentsResult(
            WorkbookModel workbookModel,
            ReactiveCommand<SaveCommandParameter, Unit> saveCommand)
        {
            this.WorkbookModel = workbookModel;
            this.SaveCommand = saveCommand;
        }

        /// <summary>
        /// Gets the workbook model.
        /// </summary>
        public WorkbookModel WorkbookModel { get; }

        /// <summary>
        /// Gets a command to invoke to save the request.
        /// </summary>
        public ReactiveCommand<SaveCommandParameter, Unit> SaveCommand { get; }
    }
}
