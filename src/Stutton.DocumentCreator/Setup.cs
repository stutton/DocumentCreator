using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Automations;
using Stutton.DocumentCreator.Fields;
using Stutton.DocumentCreator.Models.Document;
using Stutton.DocumentCreator.Models.Settings;
using Stutton.DocumentCreator.Models.Template;
using Stutton.DocumentCreator.Properties;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Services.Automations;
using Stutton.DocumentCreator.Services.Document;
using Stutton.DocumentCreator.Services.Fields;
using Stutton.DocumentCreator.Services.Image;
using Stutton.DocumentCreator.Services.Image.BuiltIn;
using Stutton.DocumentCreator.Services.Settings;
using Stutton.DocumentCreator.Services.Telemetry;
using Stutton.DocumentCreator.Services.Templates;
using Stutton.DocumentCreator.Services.Updating;
using Stutton.DocumentCreator.Services.Vsts;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels;
using Stutton.DocumentCreator.ViewModels.Dialogs;
using Stutton.DocumentCreator.ViewModels.Navigation;
using Stutton.DocumentCreator.ViewModels.Pages;
using Unity;
using Unity.Lifetime;

namespace Stutton.DocumentCreator
{
    public static class Setup
    {
        private static IUnityContainer _container;
        public static async Task DoSetup(ISnackbarMessageQueue messageQueue, bool debugging, IWindow mainWindow)
        {
            Configure(messageQueue, mainWindow);
            await LoadInitialSettings();
            await InitializeTelemetryService();
            await LoadFirstRunTemplates();
            if (!debugging)
            {
                CheckForUpdate();
            }
            LoadPages();
        }

        public static ShellViewModel GetShellViewModel()
        {
            return _container.Resolve<ShellViewModel>();
        }

        public static void Dispose()
        {
            _container.Dispose();
        }

        private static void Configure(ISnackbarMessageQueue messageQueue, IWindow mainWindow)
        {
            _container = new UnityContainer();
            _container.RegisterInstance(mainWindow, new ExternallyControlledLifetimeManager());
            _container.RegisterInstance<IContext>(new WpfContext(), new ContainerControlledLifetimeManager());
            _container.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ISettingsService, SettingsService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IVstsService, VstsService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IImageService, ImageService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ITemplatesService, TemplatesService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ITelemetryService, TelemetryService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IServiceResolver, ServiceResolver>(new ContainerControlledLifetimeManager());
            _container.RegisterInstance<IMapper>(InitializeMapper());
            _container.RegisterInstance<IFieldTemplateFactoryService>(new FieldTemplateFactoryService(t => _container.Resolve(t) as FieldTemplateModelBase));
            _container.RegisterInstance<IAutomationFactoryService>(
                new AutomationFactoryService(t => _container.Resolve(t) as AutomationModelBase));
            _container.RegisterType<IDocumentService, DocumentService>(new ContainerControlledLifetimeManager());
            _container.RegisterInstance(messageQueue, new ExternallyControlledLifetimeManager());
            _container.RegisterType<IUpdaterService, UpdaterService>(new ContainerControlledLifetimeManager());
        }

        private static Mapper InitializeMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.ConstructServicesUsing(t => _container.Resolve(t));
                cfg.AddMaps(typeof(Setup).Assembly);
                cfg.CreateMap<DocumentTemplateModel, DocumentTemplateDto>().ReverseMap();
                cfg.CreateMap<DocumentModel, DocumentDto>().ReverseMap();
            });
            mapperConfig.AssertConfigurationIsValid();
            return new Mapper(mapperConfig, t => _container.Resolve(t));
        }

        private static async Task LoadInitialSettings()
        {
            var settingsService = _container.Resolve<ISettingsService>();
            var tfsService = _container.Resolve<IVstsService>();
            var telemetryService = _container.Resolve<ITelemetryService>();

            // Load settings
            var settingsResponse = await settingsService.GetSettings();
            var settings = new SettingsModel();
            if (!settingsResponse.Success)
            {
                if (settingsResponse.Code != ResponseCode.FileNotFound)
                {
                    await DialogHost.Show(new ErrorMessageDialogViewModel(settingsResponse.Message), MainWindow.RootDialog);
                    return;
                }
            }
            else
            {
                settings = settingsResponse.Value;
            }

            // Load default settings
            var defaultSettingsResponse = await settingsService.GetDefaultSettingsAsync();
            if (!defaultSettingsResponse.Success)
            {
                if (defaultSettingsResponse.Code != ResponseCode.FileNotFound)
                {
                    await DialogHost.Show(new ErrorMessageDialogViewModel(defaultSettingsResponse.Message), MainWindow.RootDialog);
                    return;
                }
            }
            else
            {
                ApplyDefaultSettings(settings, defaultSettingsResponse.Value);
            }

            // Check TfsUrl
            if (string.IsNullOrEmpty(settings.TfsUrl))
            {
                var tfsUrlDialog = new TfsUrlDialogViewModel();
                if (!(bool)await DialogHost.Show(tfsUrlDialog, MainWindow.RootDialog))
                {
                    await DialogHost.Show(new ErrorMessageDialogViewModel("Connecting to VSTS/TFS canceled by user"), MainWindow.RootDialog);
                    return;
                }

                settings.TfsUrl = tfsUrlDialog.TfsUrl;
            }

            // Save settings so the URL is available to other services
            if (!await SaveSettingsAsync(settingsService, settings))
            {
                return;
            }

            // Check TfsDefaultCollection
            if (string.IsNullOrEmpty(settings.TfsDefaultCollection))
            {
                settings.TfsDefaultCollection = Resources.TfsDefaultCollection_DefaultValue;
            }

            if (!await SaveSettingsAsync(settingsService, settings))
            {
                return;
            }

            // Check TfsUserName
            if (string.IsNullOrEmpty(settings.TfsUserName))
            {
                var tfsProfileResponse = await tfsService.GetUserProfileAsync();
                if (!tfsProfileResponse.Success)
                {
                    await DialogHost.Show(new ErrorMessageDialogViewModel(tfsProfileResponse.Message), MainWindow.RootDialog);
                    return;
                }

                settings.TfsUserName = tfsProfileResponse.Value.Name;
                // TODO: Save profile picture
            }

            if (!await SaveSettingsAsync(settingsService, settings))
            {
                return;
            }

            // Check Application Insights key
            if (settings.SendTelemetryEnabled == null || !settings.SendTelemetryEnabled.Value || string.IsNullOrEmpty(settings.ApplicationInsightsKey))
            {
                telemetryService.Enabled = false;
            }

            // Save settings
            await SaveSettingsAsync(settingsService, settings);
        }

        private static async Task<bool> SaveSettingsAsync(ISettingsService settingsService, SettingsModel settings)
        {
            var saveSettingsResponse = await settingsService.SaveSettings(settings);
            if (!saveSettingsResponse.Success)
            {
                await DialogHost.Show(new ErrorMessageDialogViewModel(saveSettingsResponse.Message), MainWindow.RootDialog);
                return false;
            }

            return true;
        }

        private static void LoadPages()
        {
            var navigationService = _container.Resolve<INavigationService>();
            var pageVmTypes = typeof(MainWindow).Assembly.GetInheritingTypes<IPage>();
            foreach (var pageVmType in pageVmTypes)
            {
                try
                {
                    if (!(_container.Resolve(pageVmType) is IPage pageVm))
                    {
                        throw new InvalidOperationException(
                            $"Page type {pageVmType.Name} is not a valid IPage and will be ignored");
                    }

                    if (pageVm.IsOnDemandPage)
                        navigationService.AddOnDemandPage(pageVm.PageKey, () => _container.Resolve(pageVmType) as IPage);
                    else
                        navigationService.AddSidebarPage(pageVm.PageKey, pageVm);
                }
                catch (InvalidOperationException ex)
                {
                    var telemetryService = _container.Resolve<ITelemetryService>();
                    telemetryService.TrackException(ex);
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        private static async Task InitializeTelemetryService()
        {
            var telemetryService = _container.Resolve<ITelemetryService>();
            var response = await telemetryService.Initialize();
            if (!response.Success && response.Code != ResponseCode.Disabled)
            {
                await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message), MainWindow.RootDialog);
            }
        }

        private static void CheckForUpdate()
        {
            Task.Run(async () =>
            {
                var updaterService = _container.Resolve<IUpdaterService>();
                var response = await updaterService.Update();
                if (!response.Success)
                {
                    var telemetryService = _container.Resolve<ITelemetryService>();
                    telemetryService.TrackFailedResponse(response);
                }

                if (response.Value.UpdateInstalled)
                {
                    var messageQueue = _container.Resolve<ISnackbarMessageQueue>();
                    messageQueue.Enqueue($"Update {response.Value.NewVersion} installed. Restart to get new features and fixes.");
                }
            });
        }

        private static void ApplyDefaultSettings(SettingsModel currentSettings, SettingsModel defaultSettings)
        {
            var properties = typeof(SettingsModel).GetProperties();
            foreach (var property in properties)
            {
                var currentValueObj = property.GetValue(currentSettings);
                var defaultValueObj = property.GetValue(defaultSettings);
                if (currentValueObj == null && defaultValueObj != null)
                {
                    property.SetValue(currentSettings, defaultValueObj);
                }
            }
        }

        private static async Task LoadFirstRunTemplates()
        {
            var telemetryService = _container.Resolve<ITelemetryService>();
            try
            {
                var templateService = _container.Resolve<ITemplatesService>();
                var response = await templateService.ImportFirstRunTemplates();
                if (!response.Success)
                {
                    telemetryService.TrackFailedResponse(response);
                    await DialogHost.Show(new ErrorMessageDialogViewModel($"Error loading first run templates: {response.Message}"), MainWindow.RootDialog);
                }
            }
            catch (Exception ex)
            {
                telemetryService?.TrackException(ex);
                await DialogHost.Show(new ErrorMessageDialogViewModel("Error loading first run templates"), MainWindow.RootDialog);
            }
        }
    }
}
