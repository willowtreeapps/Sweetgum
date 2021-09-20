// <copyright file="RequestWorkbookItemViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Reactive;
using System.Reactive.Subjects;
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
        private readonly ObservableAsPropertyHelper<int> levelObservableAsPropertyHelper;
        private readonly BehaviorSubject<int> levelBehaviorSubject;
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

            this.levelBehaviorSubject = new BehaviorSubject<int>(CalculateLevel(workbookModel, requestModel.Id));

            this.levelObservableAsPropertyHelper =
                this.levelBehaviorSubject.ToProperty(this, viewModel => viewModel.Level);
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
        /// Gets the level of the request, which is the depth in the workbook.
        /// </summary>
        public int Level => this.levelObservableAsPropertyHelper.Value;

        /// <summary>
        /// Update a request using a new request model and new workbook model.
        /// </summary>
        /// <param name="workbookModel">The new workbook model.</param>
        /// <param name="requestModel">The new request model.</param>
        public void Update(
            WorkbookModel workbookModel,
            RequestModel requestModel)
        {
            this.Name = requestModel.Name;
            this.levelBehaviorSubject.OnNext(CalculateLevel(workbookModel, requestModel.Id));
        }

        private static int CalculateLevel(WorkbookModel workbookModel, Guid requestId)
        {
            // TODO: This seems like it could be improved, since ideally we could just use the request model to determine the path.
            workbookModel.TryGetRequest(requestId, out _, out var pathModel);

            if (pathModel == null)
            {
                throw new Exception("The request with the ID specified does not exist in the workbook.");
            }

            return pathModel.Segments.Count - 1;
        }
    }
}
