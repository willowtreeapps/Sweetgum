// <copyright file="WorkbookRequestItemsViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Avalonia.Collections;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Requests.Models;

namespace WillowTree.Sweetgum.Client.Workbooks.ViewModels
{
    /// <summary>
    /// A workbook request items view model.
    /// </summary>
    public sealed class WorkbookRequestItemsViewModel : ReactiveObject
    {
        private readonly ReactiveCommand<RequestModel, Unit> openRequestCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookRequestItemsViewModel"/> class.
        /// </summary>
        /// <param name="requests">A read-only list of <see cref="RequestModel"/>.</param>
        /// <param name="openRequestCommand">A command to invoke to open the request.</param>
        public WorkbookRequestItemsViewModel(
            IReadOnlyList<RequestModel> requests,
            ReactiveCommand<RequestModel, Unit> openRequestCommand)
        {
            this.openRequestCommand = openRequestCommand;
            this.Items = new AvaloniaList<RequestWorkbookItemViewModel>();

            // TODO: We might need a DI scope here, but this should be fine for now.
            this.Items.AddRange(requests
                .Select(r => new RequestWorkbookItemViewModel(r, this.openRequestCommand))
                .ToList());
        }

        /// <summary>
        /// Gets the request items.
        /// </summary>
        public AvaloniaList<RequestWorkbookItemViewModel> Items { get; }

        /// <summary>
        /// Update the request items (remove old and add new) as well as looping through requests and updating in-place.
        /// </summary>
        /// <param name="requests">A list of updated request models.</param>
        public void Update(IReadOnlyList<RequestModel> requests)
        {
            var existingRequests = this.Items
                .ToList();

            var removedRequests = existingRequests
                .Where(existingRequest => requests.All(r => r.GetPath() != existingRequest.OriginalPath))
                .ToList();

            var addedRequests = requests
                .Where(request => existingRequests.All(r => r.OriginalPath != request.GetPath()))
                .Select(request => new RequestWorkbookItemViewModel(request, this.openRequestCommand))
                .ToList();

            this.Items.RemoveAll(removedRequests);
            this.Items.AddRange(addedRequests);

            foreach (var item in this.Items)
            {
                item.Update(requests.First(r => r.GetPath() == item.OriginalPath));
            }
        }
    }
}
