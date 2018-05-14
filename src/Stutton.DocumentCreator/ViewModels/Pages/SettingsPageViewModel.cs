using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Settings;
using Stutton.DocumentCreator.Properties;
using Stutton.DocumentCreator.Services.Settings;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.ViewModels.Pages
{
    public class SettingsPageViewModel : PageBase
    {
        private readonly ISettingsService _settingsService;
        public const string Key = "SettingsPage";
        public const int Order = 2;
        public override string PageKey => Key;
        public override int PageOrder => Order;
        public override string Title => Resources.SettingsPage_Settings;
        public override bool IsOnDemandPage => false;

        public SettingsPageViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public override async Task NavigatedTo(object parameter)
        {
            var settingsResponse = await _settingsService.GetSettings();
            if (!settingsResponse.Success)
            {
                await DialogHost.Show(new ErrorMessageDialogViewModel(settingsResponse.Message), "RootDialog");
                return;
            }

            Settings = settingsResponse.Value;
        }

        private SettingsModel _settings;

        public SettingsModel Settings
        {
            get => _settings;
            set => Set(ref _settings, value);
        }
    }
}
