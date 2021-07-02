﻿// <copyright file="RequestBuilderViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using WillowTree.Sweetgum.Client.ViewModels;

namespace WillowTree.Sweetgum.Client.RequestBuilder.ViewModels
{
    /// <summary>
    /// The main window view model.
    /// </summary>
    public sealed class RequestBuilderViewModel : ViewModelBase
    {
        private readonly ObservableAsPropertyHelper<bool> shouldShowRequestDataTextBoxObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<HttpStatusCode> responseStatusCodeObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<string> responseContentObservableAsPropertyHelper;
        private HttpMethodViewModel? selectedHttpMethodViewModel;
        private string requestData;
        private string requestUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBuilderViewModel"/> class.
        /// </summary>
        public RequestBuilderViewModel()
        {
            this.requestUrl = string.Empty;
            this.requestData = string.Empty;

            var httpMethods = new List<HttpMethod>
            {
                HttpMethod.Get,
                HttpMethod.Post,
                HttpMethod.Put,
                HttpMethod.Patch,
                HttpMethod.Delete,
            };

            this.shouldShowRequestDataTextBoxObservableAsPropertyHelper = this
                .WhenAnyValue(viewModel => viewModel.SelectedHttpMethod)
                .Select(currentSelectedHttpMethod => currentSelectedHttpMethod != null && IsMethodWithRequestData(currentSelectedHttpMethod.HttpMethod))
                .ToProperty(this, viewModel => viewModel.ShouldShowRequestDataTextBox);

            this.HttpMethods = httpMethods.Select(httpMethod => new HttpMethodViewModel(httpMethod)).ToList();

            this.SendRequestCommand = ReactiveCommand.CreateFromTask(this.SendRequestAsync);

            this.SendRequestCommand.Subscribe();

            this.responseStatusCodeObservableAsPropertyHelper = this.SendRequestCommand
                .Select(result => result.StatusCode)
                .ToProperty(this, viewModel => viewModel.ResponseStatusCode);

            this.responseContentObservableAsPropertyHelper = this.SendRequestCommand
                .Select(result => result.ResponseContent)
                .ToProperty(this, viewModel => viewModel.ResponseContent);
        }

        /// <summary>
        /// Gets or sets the HTTP method selected.
        /// </summary>
        public HttpMethodViewModel? SelectedHttpMethod
        {
            get => this.selectedHttpMethodViewModel;
            set => this.RaiseAndSetIfChanged(ref this.selectedHttpMethodViewModel, value);
        }

        /// <summary>
        /// Gets the list of valid HTTP methods.
        /// </summary>
        public IList<HttpMethodViewModel> HttpMethods { get; }

        /// <summary>
        /// Gets or sets the HTTP request data.
        /// </summary>
        public string RequestData
        {
            get => this.requestData;
            set => this.RaiseAndSetIfChanged(ref this.requestData, value);
        }

        /// <summary>
        /// Gets or sets the HTTP request URL.
        /// </summary>
        public string RequestUrl
        {
            get => this.requestUrl;
            set => this.RaiseAndSetIfChanged(ref this.requestUrl, value);
        }

        /// <summary>
        /// Gets the HTTP request response content.
        /// </summary>
        public string ResponseContent => this.responseContentObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets the HTTP request response status code.
        /// </summary>
        public HttpStatusCode ResponseStatusCode => this.responseStatusCodeObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets a value indicating whether or not the text box for request data should be shown.
        /// </summary>
        public bool ShouldShowRequestDataTextBox => this.shouldShowRequestDataTextBoxObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets a command that sends the HTTP request.
        /// </summary>
        public ReactiveCommand<Unit, RequestResult> SendRequestCommand { get; }

        private static bool IsMethodWithRequestData(HttpMethod httpMethod)
        {
            return httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put || httpMethod == HttpMethod.Patch;
        }

        private async Task<RequestResult> SendRequestAsync(CancellationToken cancellationToken)
        {
            var httpClient = new HttpClient();
            var httpMethod = this.SelectedHttpMethod?.HttpMethod ?? HttpMethod.Get;

            var request = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = new Uri(this.RequestUrl),
            };

            if (IsMethodWithRequestData(httpMethod))
            {
                request.Content = new StringContent(this.requestData, Encoding.UTF8, "application/x-www-form-urlencoded");
            }

            var response = await httpClient.SendAsync(request, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            return new RequestResult(response.StatusCode, responseContent);
        }

        /// <summary>
        /// An HTTP request result.
        /// </summary>
        public sealed class RequestResult
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RequestResult"/> class.
            /// </summary>
            /// <param name="statusCode">An instance of <see cref="HttpStatusCode"/>.</param>
            /// <param name="responseContent">The response content.</param>
            public RequestResult(HttpStatusCode statusCode, string responseContent)
            {
                this.StatusCode = statusCode;
                this.ResponseContent = responseContent;
            }

            /// <summary>
            /// Gets the response status code.
            /// </summary>
            public HttpStatusCode StatusCode { get; }

            /// <summary>
            /// Gets the response content.
            /// </summary>
            public string ResponseContent { get; }
        }
    }
}