// <copyright file="RequestWorkbookItemViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Reactive;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Requests.Models;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.ViewModels
{
    /// <summary>
    /// A request workbook item view model.
    /// </summary>
    public sealed class RequestWorkbookItemViewModel : ReactiveObject
    {
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestWorkbookItemViewModel"/> class.
        /// </summary>
        /// <param name="workbookModel">The model of the workbook holding the request.</param>
        /// <param name="requestModel">An instance of <see cref="RequestModel"/>.</param>
        /// <param name="saveCommand">A command to invoke to save the request.</param>
        public RequestWorkbookItemViewModel(
            WorkbookModel workbookModel,
            RequestModel requestModel,
            ReactiveCommand<SaveCommandParameter, Unit> saveCommand)
        {
            this.Id = requestModel.Id;
            this.name = requestModel.Name;

            this.OpenRequestCommand = ReactiveCommand.Create(() => new OpenRequestResult(
                workbookModel,
                requestModel,
                saveCommand));
        }

        /// <summary>
        /// Gets or sets the name of the folder.
        /// </summary>
        public string Name
        {
            get => this.name;
            set => this.RaiseAndSetIfChanged(ref this.name, value);
        }

        /// <summary>
        /// Gets the open request command.
        /// </summary>
        public ReactiveCommand<Unit, OpenRequestResult> OpenRequestCommand { get; }

        /// <summary>
        /// Gets the ID of the request.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Update a request using a new request model.
        /// </summary>
        /// <param name="requestModel">The new request model.</param>
        public void Update(RequestModel requestModel)
        {
            this.Name = requestModel.Name;
        }
    }
}
