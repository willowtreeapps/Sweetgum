// <copyright file="RequestBuilderViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Media;
using ReactiveUI;
using WillowTree.Sweetgum.Client.ViewModels;

namespace WillowTree.Sweetgum.Client.RequestBuilder.ViewModels
{
    /// <summary>
    /// The main window view model.
    /// </summary>
    public sealed class RequestBuilderViewModel : ViewModelBase
    {
        private const string FormUrlEncodedContentType = "application/x-www-form-urlencoded";

        private readonly ObservableAsPropertyHelper<bool> shouldShowResponseDetailsObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<bool> shouldShowRequestDataTextBoxObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<HttpStatusCode> responseStatusCodeObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<Color> responseStatusCodeColorObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<string> responseContentObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<string> responseHeadersObservableAsPropertyHelper;
        private ContentTypeViewModel? selectedContentTypeViewModel;
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

            var defaultHttpMethod = this.HttpMethods.FirstOrDefault(m => m.HttpMethod == HttpMethod.Get);
            this.SelectedHttpMethod = defaultHttpMethod;

            this.ContentTypes = new List<ContentTypeViewModel>
            {
                new (FormUrlEncodedContentType, "Form URL Encoded"),
                new ("application/json", "JSON"),
                new ("text/plain", "Text"),
                new ("application/javascript", "Javascript"),
                new ("text/html", "HTML"),
                new ("application/xml", "XML"),
            };

            this.RequestHeaders = new AvaloniaList<RequestHeaderViewModel>();

            var removeObservable = new Subject<RequestHeaderViewModel>();
            removeObservable
                .Subscribe(requestHeaderViewModel =>
                {
                    if (!this.RequestHeaders.Contains(requestHeaderViewModel))
                    {
                        return;
                    }

                    this.RequestHeaders.Remove(requestHeaderViewModel);
                });

            this.AddRequestHeaderCommand = ReactiveCommand.Create(() => this.RequestHeaders.Add(new RequestHeaderViewModel(removeObservable)));

            // We can only execute the send request command if the request URL is valid.
            var canExecute = this
                .WhenAnyValue(viewModel => viewModel.RequestUrl)
                .Select(currentRequestUrl =>
                {
                    if (string.IsNullOrWhiteSpace(currentRequestUrl))
                    {
                        return false;
                    }

                    try
                    {
                        var unused = new Uri(currentRequestUrl, UriKind.Absolute);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });

            var statusCodeBehaviorSubject = new BehaviorSubject<HttpStatusCode>(default);
            var responseContentBehaviorSubject = new BehaviorSubject<string>(string.Empty);
            var responseHeadersBehaviorSubject = new BehaviorSubject<string>(string.Empty);
            var shouldShowResponseDetailsBehaviorSubject = new BehaviorSubject<bool>(false);

            this.SendRequestCommand = ReactiveCommand.CreateFromTask(
                async cancellationToken =>
                {
                    shouldShowResponseDetailsBehaviorSubject.OnNext(false);

                    return await this.SendRequestAsync(cancellationToken);
                },
                canExecute);

            this.SendRequestCommand
                .Subscribe(result =>
                {
                    statusCodeBehaviorSubject.OnNext(result.StatusCode);
                    responseContentBehaviorSubject.OnNext(result.ResponseContent);
                    responseHeadersBehaviorSubject.OnNext(string.Join("\n", result
                        .Headers
                        .Select(h => $"{h.HeaderName}: {h.HeaderValue}")));
                    shouldShowResponseDetailsBehaviorSubject.OnNext(true);
                });

            this.responseStatusCodeObservableAsPropertyHelper = statusCodeBehaviorSubject
                .ToProperty(this, viewModel => viewModel.ResponseStatusCode);

            this.responseStatusCodeColorObservableAsPropertyHelper = statusCodeBehaviorSubject
                .Select(currentStatusCode => currentStatusCode switch
                {
                    >= HttpStatusCode.Continue and < HttpStatusCode.OK => Color.FromRgb(0, 0, 255),
                    >= HttpStatusCode.OK and < HttpStatusCode.Ambiguous => Color.FromRgb(68, 100, 18),
                    >= HttpStatusCode.Ambiguous and < HttpStatusCode.BadRequest => Color.FromRgb(255, 255, 0),
                    >= HttpStatusCode.BadRequest and < HttpStatusCode.InternalServerError => Color.FromRgb(255, 140, 0),
                    _ => Color.FromRgb(255, 0, 0),
                })
                .ToProperty(this, viewModel => viewModel.ResponseStatusCodeColor);

            this.responseContentObservableAsPropertyHelper = responseContentBehaviorSubject
                .ToProperty(this, viewModel => viewModel.ResponseContent);

            this.responseHeadersObservableAsPropertyHelper = responseHeadersBehaviorSubject
                .ToProperty(this, viewModel => viewModel.ResponseHeaders);

            this.shouldShowResponseDetailsObservableAsPropertyHelper = shouldShowResponseDetailsBehaviorSubject
                .ToProperty(this, viewModel => viewModel.ShouldShowResponseDetails);

            this.SendRequestCommand.ThrownExceptions
                .Subscribe(_ =>
                {
                    statusCodeBehaviorSubject.OnNext(default);
                    responseContentBehaviorSubject.OnNext(string.Empty);
                    shouldShowResponseDetailsBehaviorSubject.OnNext(true);

                    // TODO: Show the exception error.
                });
        }

        /// <summary>
        /// Gets or sets the content type selected.
        /// </summary>
        public ContentTypeViewModel? SelectedContentType
        {
            get => this.selectedContentTypeViewModel;
            set => this.RaiseAndSetIfChanged(ref this.selectedContentTypeViewModel, value);
        }

        /// <summary>
        /// Gets the valid list of content types.
        /// </summary>
        public List<ContentTypeViewModel> ContentTypes { get; }

        /// <summary>
        /// Gets the observable collection of request headers.
        /// </summary>
        public AvaloniaList<RequestHeaderViewModel> RequestHeaders { get; }

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
        /// Gets the HTTP request response headers.
        /// </summary>
        public string ResponseHeaders => this.responseHeadersObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets the HTTP request response status code.
        /// </summary>
        public HttpStatusCode ResponseStatusCode => this.responseStatusCodeObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets the HTTP request response status code color.
        /// </summary>
        public Color ResponseStatusCodeColor => this.responseStatusCodeColorObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets a value indicating whether or not the text box for request data should be shown.
        /// </summary>
        public bool ShouldShowRequestDataTextBox => this.shouldShowRequestDataTextBoxObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets a value indicating whether or not the response details should be shown.
        /// </summary>
        public bool ShouldShowResponseDetails => this.shouldShowResponseDetailsObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets a command that adds a request header.
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddRequestHeaderCommand { get; }

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
                RequestUri = new Uri(this.RequestUrl, UriKind.Absolute),
            };

            if (IsMethodWithRequestData(httpMethod))
            {
                var mediaType = string.IsNullOrWhiteSpace(this.SelectedContentType?.ContentType)
                    ? FormUrlEncodedContentType
                    : this.SelectedContentType?.ContentType;

                request.Content = new StringContent(
                    this.requestData,
                    Encoding.UTF8,
                    mediaType);
            }

            foreach (var requestHeader in this.RequestHeaders)
            {
                request.Headers.TryAddWithoutValidation(requestHeader.Name!, requestHeader.Value!);
            }

            var response = await httpClient.SendAsync(request, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            var responseHeaders = new List<(string HeaderName, string HeaderValue)>();

            foreach (var responseHeader in response.Headers)
            {
                foreach (var responseHeaderValue in responseHeader.Value)
                {
                    responseHeaders.Add((responseHeader.Key, responseHeaderValue));
                }
            }

            return new RequestResult(
                response.StatusCode,
                responseContent,
                responseHeaders);
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
            /// <param name="headers">A read-only list of response headers.</param>
            public RequestResult(
                HttpStatusCode statusCode,
                string responseContent,
                IReadOnlyList<(string HeaderName, string HeaderValue)> headers)
            {
                this.StatusCode = statusCode;
                this.ResponseContent = responseContent;
                this.Headers = headers;
            }

            /// <summary>
            /// Gets the response status code.
            /// </summary>
            public HttpStatusCode StatusCode { get; }

            /// <summary>
            /// Gets the response content.
            /// </summary>
            public string ResponseContent { get; }

            /// <summary>
            /// Gets the response headers dictionary.
            /// </summary>
            public IReadOnlyList<(string HeaderName, string HeaderValue)> Headers { get; }
        }
    }
}
