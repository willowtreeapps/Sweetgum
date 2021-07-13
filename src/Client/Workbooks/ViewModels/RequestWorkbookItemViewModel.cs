// <copyright file="RequestWorkbookItemViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using ReactiveUI;
using WillowTree.Sweetgum.Client.Requests.Models;

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
        /// <param name="requestModel">An instance of <see cref="RequestModel"/>.</param>
        public RequestWorkbookItemViewModel(RequestModel requestModel)
        {
            this.name = requestModel.Name;
        }

        /// <summary>
        /// Gets or sets the name of the folder.
        /// </summary>
        public string Name
        {
            get => this.name;
            set => this.RaiseAndSetIfChanged(ref this.name, value);
        }
    }
}
