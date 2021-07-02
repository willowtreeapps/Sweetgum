// <copyright file="RequestHeaderViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Reactive;
using ReactiveUI;
using WillowTree.Sweetgum.Client.ViewModels;

namespace WillowTree.Sweetgum.Client.RequestBuilder.ViewModels
{
    /// <summary>
    /// The request header view model.
    /// </summary>
    public sealed class RequestHeaderViewModel : ViewModelBase
    {
        private string? name;
        private string? value;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestHeaderViewModel"/> class.
        /// </summary>
        /// <param name="removeObserver">An observer to notify when you want to remove the request header.</param>
        public RequestHeaderViewModel(IObserver<RequestHeaderViewModel> removeObserver)
        {
            this.RemoveCommand = ReactiveCommand.Create(() => removeObserver.OnNext(this));
        }

        /// <summary>
        /// Gets or sets the name of the request header.
        /// </summary>
        public string? Name
        {
            get => this.name;
            set => this.RaiseAndSetIfChanged(ref this.name, value);
        }

        /// <summary>
        /// Gets or sets the value of the request header.
        /// </summary>
        public string? Value
        {
            get => this.value;
            set => this.RaiseAndSetIfChanged(ref this.value, value);
        }

        /// <summary>
        /// Gets the command to remove the request header.
        /// </summary>
        public ReactiveCommand<Unit, Unit> RemoveCommand { get; }
    }
}
