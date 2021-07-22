// <copyright file="OpenRequestResult.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Reactive;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Requests.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.Models
{
    /// <summary>
    /// The result of the open request command.
    /// </summary>
    public sealed class OpenRequestResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenRequestResult"/> class.
        /// </summary>
        /// <param name="workbookModel">An instance of <see cref="WorkbookModel"/>.</param>
        /// <param name="requestModel">An instance of <see cref="RequestModel"/>.</param>
        /// <param name="saveCommand">A command to invoke to save the request.</param>
        public OpenRequestResult(
            WorkbookModel workbookModel,
            RequestModel requestModel,
            ReactiveCommand<SaveCommandParameter, Unit> saveCommand)
        {
            this.WorkbookModel = workbookModel;
            this.RequestModel = requestModel;
            this.SaveCommand = saveCommand;
        }

        /// <summary>
        /// Gets the workbook model.
        /// </summary>
        public WorkbookModel WorkbookModel { get; }

        /// <summary>
        /// Gets the request model.
        /// </summary>
        public RequestModel RequestModel { get; }

        /// <summary>
        /// Gets a command to invoke to save the request.
        /// </summary>
        public ReactiveCommand<SaveCommandParameter, Unit> SaveCommand { get; }
    }
}
