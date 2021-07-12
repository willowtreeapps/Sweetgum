// <copyright file="SettingsViewModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using WillowTree.Sweetgum.Client.Settings.Models;
using WillowTree.Sweetgum.Client.Settings.Services;

namespace WillowTree.Sweetgum.Client.Settings.ViewModels
{
    /// <summary>
    /// The main window view model.
    /// </summary>
    public sealed class SettingsViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<bool> manualProxyIsVisibleObservableAsPropertyHelper;
        private string proxyHost;
        private string proxyPort;
        private ProxyOptionViewModel proxyOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        /// <param name="settingsManager">An instance of <see cref="SettingsManager"/>.</param>
        public SettingsViewModel(SettingsManager settingsManager)
        {
            var useSystemSettingsProxyOption = new ProxyOptionViewModel(
                Models.ProxyOption.UseSystemSettings,
                "Use System Settings");

            this.ProxyOptions = new List<ProxyOptionViewModel>
            {
                useSystemSettingsProxyOption,
                new(Models.ProxyOption.Manual, "Manual"),
                new(Models.ProxyOption.Disabled, "Disabled"),
            };

            var currentSettings = settingsManager.CurrentSettings;

            this.proxyHost = currentSettings.ProxyHost;
            this.proxyPort = currentSettings.ProxyPort.ToString();
            this.proxyOption = this.ProxyOptions.FirstOrDefault(o => o.ProxyOption == currentSettings.ProxyOption) ?? useSystemSettingsProxyOption;

            this.manualProxyIsVisibleObservableAsPropertyHelper = this.WhenAnyValue(viewModel => viewModel.ProxyOption)
                .Select(o => o.ProxyOption == Models.ProxyOption.Manual)
                .ToProperty(this, viewModel => viewModel.ManualProxyIsVisible);

            this.SaveCommand = ReactiveCommand.Create(() =>
            {
                // TODO: Handle a bad port number (int.Parse exception).
                settingsManager.Save(new SettingsModel(this.ProxyHost, int.Parse(this.ProxyPort), this.ProxyOption.ProxyOption));
            });

            this.CancelCommand = ReactiveCommand.Create(() => { });
        }

        /// <summary>
        /// Gets or sets the proxy host.
        /// </summary>
        public string ProxyHost
        {
            get => this.proxyHost;
            set => this.RaiseAndSetIfChanged(ref this.proxyHost, value);
        }

        /// <summary>
        /// Gets or sets the proxy port.
        /// </summary>
        public string ProxyPort
        {
            get => this.proxyPort;
            set => this.RaiseAndSetIfChanged(ref this.proxyPort, value);
        }

        /// <summary>
        /// Gets or sets the proxy option.
        /// </summary>
        public ProxyOptionViewModel ProxyOption
        {
            get => this.proxyOption;
            set => this.RaiseAndSetIfChanged(ref this.proxyOption, value);
        }

        /// <summary>
        /// Gets a value indicating whether or not the proxy host and port text boxes should be visible.
        /// </summary>
        public bool ManualProxyIsVisible => this.manualProxyIsVisibleObservableAsPropertyHelper.Value;

        /// <summary>
        /// Gets the list of valid proxy options.
        /// </summary>
        public IList<ProxyOptionViewModel> ProxyOptions { get; }

        /// <summary>
        /// Gets a command that saves the settings.
        /// </summary>
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        /// <summary>
        /// Gets a command that cancels the changes to the settings.
        /// </summary>
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    }
}
