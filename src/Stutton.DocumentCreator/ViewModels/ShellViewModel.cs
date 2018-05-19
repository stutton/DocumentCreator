using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Settings;
using Stutton.DocumentCreator.Properties;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Services.Settings;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;
using Stutton.DocumentCreator.ViewModels.Navigation;
using Stutton.DocumentCreator.ViewModels.Pages;

namespace Stutton.DocumentCreator.ViewModels
{
    public class ShellViewModel : Observable
    {
        private readonly ISettingsService _settingsService;
        private readonly ITfsService _tfsService;

        public ShellViewModel(INavigationService navigationService, ISettingsService settingsService, ITfsService tfsService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _tfsService = tfsService ?? throw new ArgumentNullException(nameof(tfsService));
            Navigator = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public INavigationService Navigator { get; }

        private bool _isSideBarShown;

        public bool IsSideBarShown
        {
            get => _isSideBarShown;
            set => Set(ref _isSideBarShown, value);
        }

        private ICommand _toggleMenuCommand;
        public ICommand ToggleMenuCommand => _toggleMenuCommand ?? (_toggleMenuCommand = new RelayCommand(() => IsSideBarShown = !IsSideBarShown));

        public async Task LoadAsync()
        {
            await LoadInitialSettings();
            await Navigator.NavigateTo(DocumentsPageViewModel.Key);
        }

        private async Task LoadInitialSettings()
        {
            // Load settings
            var settingsResponse = await _settingsService.GetSettings();
            var settings = new SettingsModel();
            if (!settingsResponse.Success)
            {
                if (settingsResponse.Code != ResponseCode.FileNotFound)
                {
                    await DialogHost.Show(new ErrorMessageDialogViewModel(settingsResponse.Message), "RootDialog");
                    return;
                }
            }
            else
            {
                settings = settingsResponse.Value;
            }

            // Load settings transform
            var transformResponse = await _settingsService.GetSettingsTransformAsync();
            var transform = new SettingsTransformModel();
            if (!transformResponse.Success)
            {
                if (transformResponse.Code != ResponseCode.FileNotFound)
                {
                    await DialogHost.Show(new ErrorMessageDialogViewModel(transformResponse.Message), "RootDialog");
                    return;
                }
            }
            else
            {
                transform = transformResponse.Value;
            }

            // Load TfsUrl
            if (string.IsNullOrEmpty(settings.TfsUrl))
            {
                if (string.IsNullOrEmpty(transform.TfsUrl))
                {
                    var tfsUrlDialog = new TfsUrlDialogViewModel();
                    if (!(bool) await DialogHost.Show(tfsUrlDialog, "RootDialog"))
                    {
                        await DialogHost.Show(new ErrorMessageDialogViewModel("Connecting to VSTS/TFS canceled by user"), "RootDialog");
                        return;
                    }

                    settings.TfsUrl = tfsUrlDialog.TfsUrl;
                }
                else
                {
                    settings.TfsUrl = transform.TfsUrl;
                }
            }

            // Load TfsDefaultCollection
            if (string.IsNullOrEmpty(settings.TfsDefaultCollection))
            {
                settings.TfsDefaultCollection = string.IsNullOrEmpty(transform.TfsDefaultCollection)
                    ? Resources.TfsDefaultCollection_DefaultValue
                    : transform.TfsDefaultCollection;
            }

            // Load TfsUserName
            if (string.IsNullOrEmpty(settings.TfsUserName))
            {
                var tfsProfileResponse = await _tfsService.GetUserProfileAsync();
                if (!tfsProfileResponse.Success)
                {
                    await DialogHost.Show(new ErrorMessageDialogViewModel(tfsProfileResponse.Message), "RootDialog");
                    return;
                }

                settings.TfsUserName = tfsProfileResponse.Value.Name;
                // TODO: Save profile picture
            }

            // Load WorkItemQuery
            if (!settings.WorkItemQuery.Expressions.Any())
            {
                if (transform.WorkItemQuery.Expressions.Any())
                {
                    settings.WorkItemQuery = transform.WorkItemQuery;
                }
            }

            // Save settings
            var saveSettingsResponse = await _settingsService.SaveSettings(settings);
            if (!saveSettingsResponse.Success)
            {
                await DialogHost.Show(new ErrorMessageDialogViewModel(saveSettingsResponse.Message), "RootDialog");
            }
        }
    }
}
