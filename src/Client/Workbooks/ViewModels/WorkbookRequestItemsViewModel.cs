// <copyright file="WorkbookRequestItemsViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Avalonia.Collections;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Requests.Models;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.ViewModels
{
    /// <summary>
    /// A workbook request items view model.
    /// </summary>
    public sealed class WorkbookRequestItemsViewModel : ReactiveObject
    {
        private readonly ReactiveCommand<SaveCommandParameter, Unit> saveCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookRequestItemsViewModel"/> class.
        /// </summary>
        /// <param name="workbookModel">The workbook model holding the requests.</param>
        /// <param name="requests">A read-only list of <see cref="RequestModel"/>.</param>
        /// <param name="saveCommand">A command to invoke to save the request.</param>
        public WorkbookRequestItemsViewModel(
            WorkbookModel workbookModel,
            IReadOnlyList<RequestModel> requests,
            ReactiveCommand<SaveCommandParameter, Unit> saveCommand)
        {
            this.saveCommand = saveCommand;
            this.Items = new AvaloniaList<RequestWorkbookItemViewModel>();

            // TODO: We might need a DI scope here, but this should be fine for now.
            this.Items.AddRange(requests
                .Select(r => new RequestWorkbookItemViewModel(workbookModel, r, saveCommand))
                .ToList());
        }

        /// <summary>
        /// Gets the request items.
        /// </summary>
        public AvaloniaList<RequestWorkbookItemViewModel> Items { get; }

        /// <summary>
        /// Update the request items (remove old and add new) as well as looping through requests and updating in-place.
        /// </summary>
        /// <param name="workbookModel">A new workbook model.</param>
        /// <param name="requests">A list of updated request models.</param>
        public void Update(
            WorkbookModel workbookModel,
            IReadOnlyList<RequestModel> requests)
        {
            var existingRequests = this.Items
                .ToList();

            var removedRequests = existingRequests
                .Where(existingRequest => requests.All(r => r.Id != existingRequest.Id))
                .ToList();

            var addedRequests = requests
                .Where(request => existingRequests.All(r => r.Id != request.Id))
                .Select(request => new RequestWorkbookItemViewModel(workbookModel, request, this.saveCommand))
                .ToList();

            this.Items.RemoveAll(removedRequests);
            this.Items.AddRange(addedRequests);

            foreach (var item in this.Items)
            {
                item.Update(
                    workbookModel,
                    requests.First(r => r.Id == item.Id));
            }
        }
    }
}
