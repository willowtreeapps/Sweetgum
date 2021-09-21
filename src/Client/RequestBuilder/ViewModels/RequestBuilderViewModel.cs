// <copyright file="RequestBuilderViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Media;
using ExhaustiveMatching;
using Newtonsoft.Json;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Requests.Models;
using WillowTree.Sweetgum.Client.Settings.Models;
using WillowTree.Sweetgum.Client.Settings.Services;
using WillowTree.Sweetgum.Client.Workbooks.Models;

namespace WillowTree.Sweetgum.Client.RequestBuilder.ViewModels
{
    /// <summary>
    /// The main window view model.
    /// </summary>
    public sealed class RequestBuilderViewModel : ReactiveObject, IActivatableViewModel
    {
        private const string FormUrlEncodedContentType = "application/x-www-form-urlencoded";

        private readonly ObservableAsPropertyHelper<PathModel> originalPathObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<bool> canSaveObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<string> displayResponseTextObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<bool> shouldShowResponseDetailsObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<bool> shouldShowRequestDataTextBoxObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<string> responseStatusCodeObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<SolidColorBrush> responseStatusCodeColorObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<string> responseContentObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<string> responseHeadersObservableAsPropertyHelper;
        private readonly ObservableAsPropertyHelper<string> responseTimeObservableAsPropertyHelper;
        private readonly Subject<RequestModel> requestModelSubject;
        private readonly Subject<RequestHeaderViewModel> removeSubject;
        private readonly SettingsManager settingsManager;
        private string name;
        private bool isPrettyJsonEnabled;
        private ContentTypeViewModel? selectedContentTypeViewModel;
        private HttpMethodViewModel? selectedHttpMethodViewModel;
        private string requestData;
        private string requestUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBuilderViewModel"/> class.
        /// </summary>
        /// <param name="requestModel">An instance of <see cref="RequestModel"/>.</param>
        /// <param name="settingsManager">An instance of <see cref="settingsManager"/>.</param>
        /// <param name="saveCommand">An observer to notify when the request is saved.</param>
        public RequestBuilderViewModel(
            RequestModel requestModel,
            SettingsManager settingsManager,
            ReactiveCommand<SaveCommandParameter, Unit> saveCommand)
        {
            this.settingsManager = settingsManager;
            this.Activator = new ViewModelActivator();
            this.requestModelSubject = new Subject<RequestModel>();
            this.name = string.Empty;
            this.requestData = string.Empty;
            this.requestUrl = string.Empty;

            this.HttpMethods = InitializeHttpMethods();
            this.RequestHeaders = new AvaloniaList<RequestHeaderViewModel>();
            this.ContentTypes = InitializeContentTypes();

            this.removeSubject = new Subject<RequestHeaderViewModel>();

            this.originalPathObservableAsPropertyHelper = this.requestModelSubject
                .Select(newRequestModel => newRequestModel.GetPath())
                .ToProperty(this, viewModel => viewModel.OriginalPath);

            this.shouldShowRequestDataTextBoxObservableAsPropertyHelper = this
                .WhenAnyValue(viewModel => viewModel.SelectedHttpMethod)
                .Select(currentSelectedHttpMethod => currentSelectedHttpMethod != null && IsMethodWithRequestData(currentSelectedHttpMethod.HttpMethod))
                .ToProperty(this, viewModel => viewModel.ShouldShowRequestDataTextBox);

            this.removeSubject
                .Subscribe(requestHeaderViewModel =>
                {
                    if (!this.RequestHeaders.Contains(requestHeaderViewModel))
                    {
                        return;
                    }

                    this.RequestHeaders.Remove(requestHeaderViewModel);
                });

            this.AddRequestHeaderCommand = ReactiveCommand.Create(() => this.RequestHeaders.Add(new RequestHeaderViewModel(this.removeSubject)));

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
            var responseTimeBehaviorSubject = new BehaviorSubject<double>(default);
            var shouldShowResponseDetailsBehaviorSubject = new BehaviorSubject<bool>(false);

            var stopwatch = new Stopwatch();

            this.SendRequestCommand = ReactiveCommand.CreateFromTask(
                async cancellationToken =>
                {
                    shouldShowResponseDetailsBehaviorSubject.OnNext(false);

                    stopwatch.Reset();
                    stopwatch.Start();

                    return await this.SendRequestAsync(cancellationToken);
                },
                canExecute);

            this.SendRequestCommand
                .Subscribe(result =>
                {
                    stopwatch.Stop();
                    statusCodeBehaviorSubject.OnNext(result.StatusCode);
                    responseContentBehaviorSubject.OnNext(result.ResponseContent);
                    responseHeadersBehaviorSubject.OnNext(string.Join("\n", result
                        .Headers
                        .Select(h => $"{h.Name}: {h.Value}")));
                    responseTimeBehaviorSubject.OnNext(stopwatch.Elapsed.TotalMilliseconds);
                    shouldShowResponseDetailsBehaviorSubject.OnNext(true);
                });

            this.responseStatusCodeObservableAsPropertyHelper = statusCodeBehaviorSubject
                .Select(responseCode => (int)responseCode + " " + responseCode)
                .ToProperty(this, viewModel => viewModel.ResponseStatusCode);

            this.responseStatusCodeColorObservableAsPropertyHelper = statusCodeBehaviorSubject
                .Select(currentStatusCode =>
                {
                    var color = currentStatusCode switch
                    {
                        >= HttpStatusCode.Continue and < HttpStatusCode.OK => Color.FromRgb(0, 0, 255),
                        >= HttpStatusCode.OK and < HttpStatusCode.Ambiguous => Color.FromRgb(68, 100, 18),
                        >= HttpStatusCode.Ambiguous and < HttpStatusCode.BadRequest => Color.FromRgb(255, 255, 0),
                        >= HttpStatusCode.BadRequest and < HttpStatusCode.InternalServerError => Color.FromRgb(255, 140, 0),
                        _ => Color.FromRgb(255, 0, 0),
                    };

                    return new SolidColorBrush(color);
                })
                .ToProperty(this, viewModel => viewModel.ResponseStatusCodeColor);

            this.responseContentObservableAsPropertyHelper = responseContentBehaviorSubject
                .ToProperty(this, viewModel => viewModel.ResponseContent);

            this.responseHeadersObservableAsPropertyHelper = responseHeadersBehaviorSubject
                .ToProperty(this, viewModel => viewModel.ResponseHeaders);

            this.responseTimeObservableAsPropertyHelper = responseTimeBehaviorSubject
                .Select(responseTime => $"{Math.Round(responseTime, 2, MidpointRounding.AwayFromZero)}ms")
                .ToProperty(this, viewModel => viewModel.ResponseTime);

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

            this.displayResponseTextObservableAsPropertyHelper = this.WhenAnyValue(
                    viewModel => viewModel.ResponseContent,
                    viewModel => viewModel.IsPrettyJsonEnabled)
                .Select(tuple =>
                {
                    var (responseContent, prettyJsonEnabled) = tuple;
                    if (!prettyJsonEnabled)
                    {
                        return responseContent;
                    }

                    var parsedJson = JsonConvert.DeserializeObject(responseContent ?? string.Empty);
                    return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
                })
                .ToProperty(this, viewModel => viewModel.DisplayResponseText);

            this.SaveCommand = saveCommand;

            this.canSaveObservableAsPropertyHelper = saveCommand.CanExecute
                .ToProperty(this, viewModel => viewModel.CanSave);

            this.WhenActivated(disposables =>
            {
                this.canSaveObservableAsPropertyHelper.DisposeWith(disposables);
            });

            this.Update(requestModel);
        }

        /// <summary>
        /// Gets a value indicating whether or not the request can be saved.
        /// </summary>
        public bool CanSave => this.canSaveObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets the display response text.
        /// </summary>
        public string DisplayResponseText => this.displayResponseTextObservableAsPropertyHelper.Value;

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
        /// Gets or sets a value indicating whether or not the response should be prettified.
        /// </summary>
        public bool IsPrettyJsonEnabled
        {
            get => this.isPrettyJsonEnabled;
            set => this.RaiseAndSetIfChanged(ref this.isPrettyJsonEnabled, value);
        }

        /// <summary>
        /// Gets or sets the request name.
        /// </summary>
        public string Name
        {
            get => this.name;
            set => this.RaiseAndSetIfChanged(ref this.name, value);
        }

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
        /// Gets the HTTP request response time.
        /// </summary>
        public string ResponseTime => this.responseTimeObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets the HTTP request response status code.
        /// </summary>
        public string ResponseStatusCode => this.responseStatusCodeObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets the HTTP request response status code color.
        /// </summary>
        public SolidColorBrush ResponseStatusCodeColor => this.responseStatusCodeColorObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets a value indicating whether or not the text box for request data should be shown.
        /// </summary>
        public bool ShouldShowRequestDataTextBox => this.shouldShowRequestDataTextBoxObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets a value indicating whether or not the response details should be shown.
        /// </summary>
        public bool ShouldShowResponseDetails => this.shouldShowResponseDetailsObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets the original path.
        /// </summary>
        public PathModel OriginalPath => this.originalPathObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets a command that adds a request header.
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddRequestHeaderCommand { get; }

        /// <summary>
        /// Gets a command that saves the HTTP request.
        /// </summary>
        public ReactiveCommand<SaveCommandParameter, Unit> SaveCommand { get; }

        /// <summary>
        /// Gets a command that sends the HTTP request.
        /// </summary>
        public ReactiveCommand<Unit, RequestResult> SendRequestCommand { get; }

        /// <summary>
        /// Gets the view model activator.
        /// </summary>
        public ViewModelActivator Activator { get; }

        /// <summary>
        /// Constructs an instance of <see cref="RequestModel"/> from the view model.
        /// </summary>
        /// <returns>An instance of <see cref="RequestModel"/>.</returns>
        public RequestModel ToModel()
        {
            return new(
                this.Name,
                this.OriginalPath.GetParent(),
                this.CalculateCurrentHttpMethod(),
                this.RequestUrl,
                this.RequestHeaders.Select(requestHeader => requestHeader.ToModel()).ToList(),
                this.SelectedContentType?.ContentType ?? string.Empty,
                this.RequestData);
        }

        /// <summary>
        /// Update the view model using an instance of <see cref="RequestModel"/>.
        /// </summary>
        /// <param name="requestModel">An instance of <see cref="RequestModel"/>.</param>
        public void Update(RequestModel requestModel)
        {
            this.name = requestModel.Name;
            this.requestUrl = requestModel.RequestUrl;
            this.requestData = requestModel.RequestData ?? string.Empty;
            this.requestModelSubject.OnNext(requestModel);

            var defaultHttpMethod = this.HttpMethods.FirstOrDefault(m => m.HttpMethod == HttpMethod.Get);
            this.SelectedHttpMethod = this.HttpMethods.FirstOrDefault(m => requestModel.HttpMethod == m.HttpMethod) ?? defaultHttpMethod;

            this.RequestHeaders.Clear();
            this.RequestHeaders.AddRange(requestModel.RequestHeaders
                .Select(h => new RequestHeaderViewModel(this.removeSubject)
                {
                    Name = h.Name,
                    Value = h.Value,
                })
                .ToList());

            this.SelectedContentType = this.ContentTypes.FirstOrDefault(t => t.ContentType == requestModel.ContentType);
        }

        private static List<ContentTypeViewModel> InitializeContentTypes()
        {
            return new List<ContentTypeViewModel>
            {
                new(FormUrlEncodedContentType, "Form URL Encoded"),
                new("application/json", "JSON"),
                new("text/plain", "Text"),
                new("application/javascript", "Javascript"),
                new("text/html", "HTML"),
                new("application/xml", "XML"),
            };
        }

        private static List<HttpMethodViewModel> InitializeHttpMethods()
        {
            var httpMethods = new List<HttpMethod>
            {
                HttpMethod.Get,
                HttpMethod.Post,
                HttpMethod.Put,
                HttpMethod.Patch,
                HttpMethod.Delete,
            };

            return httpMethods.Select(httpMethod => new HttpMethodViewModel(httpMethod)).ToList();
        }

        private static bool IsMethodWithRequestData(HttpMethod httpMethod)
        {
            return httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put || httpMethod == HttpMethod.Patch;
        }

        private async Task<RequestResult> SendRequestAsync(CancellationToken cancellationToken)
        {
            var handler = new HttpClientHandler();

            switch (this.settingsManager.CurrentSettings.ProxyOption)
            {
                case ProxyOption.UseSystemSettings:
                    handler.UseProxy = true;
                    break;
                case ProxyOption.Manual:
                    handler.UseProxy = true;
                    handler.Proxy = new WebProxy(
                        this.settingsManager.CurrentSettings.ProxyHost,
                        this.settingsManager.CurrentSettings.ProxyPort);
                    break;
                case ProxyOption.Disabled:
                    handler.UseProxy = false;
                    break;
                default:
                    throw ExhaustiveMatch.Failed();
            }

            var httpClient = new HttpClient(handler);
            var httpMethod = this.CalculateCurrentHttpMethod();

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
                request.Headers.TryAddWithoutValidation(requestHeader.Name, requestHeader.Value);
            }

            var response = await httpClient.SendAsync(request, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            var responseHeaders = new List<RequestHeaderModel>();

            foreach (var responseHeader in response.Headers)
            {
                foreach (var responseHeaderValue in responseHeader.Value)
                {
                    responseHeaders.Add(new RequestHeaderModel(responseHeader.Key, responseHeaderValue));
                }
            }

            return new RequestResult(
                response.StatusCode,
                responseContent,
                responseHeaders);
        }

        private HttpMethod CalculateCurrentHttpMethod()
        {
            return this.SelectedHttpMethod?.HttpMethod ?? HttpMethod.Get;
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
                IReadOnlyList<RequestHeaderModel> headers)
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
            public IReadOnlyList<RequestHeaderModel> Headers { get; }
        }
    }
}
