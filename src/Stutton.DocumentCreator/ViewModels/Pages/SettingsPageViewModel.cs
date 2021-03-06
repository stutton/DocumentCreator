﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Settings;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Properties;
using Stutton.DocumentCreator.Services.Settings;
using Stutton.DocumentCreator.Services.Telemetry;
using Stutton.DocumentCreator.Services.Vsts;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;
using Stutton.DocumentCreator.ViewModels.Navigation;
using Mdx = MaterialDesignExtensions.Themes;

namespace Stutton.DocumentCreator.ViewModels.Pages
{
    public class SettingsPageViewModel : PageBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IVstsService _vstsService;
        private readonly ISnackbarMessageQueue _messageQueue;
        private readonly ITelemetryService _telemetryService;
        public const string Key = "SettingsPage";
        public const int Order = 3;
        public override string PageKey => Key;
        public override int PageOrder => Order;
        public override string Title => Resources.SettingsPage_Settings;
        public override bool IsOnDemandPage => false;
        private Mdx.PaletteHelper _paletteHelper;

        public SettingsPageViewModel(ISettingsService settingsService, 
                                     IVstsService vstsService, 
                                     ISnackbarMessageQueue messageQueue, 
                                     ITelemetryService telemetryService,
                                     INavigationService navigationService)
        :base(navigationService)
        {
            _settingsService = settingsService;
            _vstsService = vstsService;
            _messageQueue = messageQueue;
            _telemetryService = telemetryService;
            _paletteHelper = new Mdx.PaletteHelper();
        }

        private bool _darkThemeEnabled;
        public bool DarkThemeEnabled
        {
            get => _darkThemeEnabled;
            set => Set(ref _darkThemeEnabled, value);
        }

        #region Save Command

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(async () => await SaveAsync()));

        private async Task SaveAsync()
        {
            var response = await _settingsService.SaveSettings(Settings);
            if (!response.Success)
            {
                _telemetryService.TrackFailedResponse(response);
                await DialogHost.Show(new ErrorMessageDialogViewModel($"Failed to save settings: {response.Message}", _telemetryService.SessionId), MainWindow.RootDialog);
                return;
            }
            _messageQueue.Enqueue("Settings saved");
        }

        #endregion


        #region ToggleDarkTheme Command

        private ICommand _toggleDarkThemeCommand;
        public ICommand ToggleDarkThemeCommand => _toggleDarkThemeCommand ?? (_toggleDarkThemeCommand = new RelayCommand<bool>(ToggleDarkTheme));

        private void ToggleDarkTheme(bool enabled)
        {
            _paletteHelper.SetLightDark(enabled);
            Properties.Settings.Default.DarkThemeEnabled = enabled;
            Properties.Settings.Default.Save();
        }

        #endregion

        public override async Task NavigatedTo(object parameter)
        {
            _telemetryService.TrackPageView(Key);

            var settingsResponse = await _settingsService.GetSettings();
            if (!settingsResponse.Success)
            {
                _telemetryService.TrackFailedResponse(settingsResponse);
                await DialogHost.Show(new ErrorMessageDialogViewModel(settingsResponse.Message, _telemetryService.SessionId), MainWindow.RootDialog);
                return;
            }

            Settings = settingsResponse.Value;
            DarkThemeEnabled = Properties.Settings.Default.DarkThemeEnabled;
        }

        private SettingsModel _settings;

        public SettingsModel Settings
        {
            get => _settings;
            set => Set(ref _settings, value);
        }
    }
}
