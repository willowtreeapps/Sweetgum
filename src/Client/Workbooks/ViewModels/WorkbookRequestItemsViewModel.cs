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
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookRequestItemsViewModel"/> class.
        /// </summary>
        /// <param name="requests">A read-only list of <see cref="RequestModel"/>.</param>
        /// <param name="saveCommand">A command to invoke to save the request.</param>
        public WorkbookRequestItemsViewModel(
            IReadOnlyList<RequestModel> requests,
            ReactiveCommand<SaveCommandParameter, Unit> saveCommand)
        {
            this.Items = new AvaloniaList<RequestWorkbookItemViewModel>();

            // TODO: We might need a DI scope here, but this should be fine for now.
            this.Items.AddRange(requests
                .Select(r => new RequestWorkbookItemViewModel(r, saveCommand))
                .ToList());
        }

        /// <summary>
        /// Gets the request items.
        /// </summary>
        public AvaloniaList<RequestWorkbookItemViewModel> Items { get; }
    }
}
