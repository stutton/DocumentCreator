using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Documents.Automations;
using Stutton.DocumentCreator.Models.Documents.Fields;
using Stutton.DocumentCreator.Services.Automations;
using Stutton.DocumentCreator.Services.Documents;
using Stutton.DocumentCreator.Services.Fields;
using Stutton.DocumentCreator.Services.Settings;
using Stutton.DocumentCreator.Services.Telemetry;
using Stutton.DocumentCreator.Services.Tfs;
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
        public static void DoSetup(ISnackbarMessageQueue messageQueue)
        {
            Configure(messageQueue);
            InitializeTelemetryService();
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

        private static void Configure(ISnackbarMessageQueue messageQueue)
        {
            _container = new UnityContainer();
            _container.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ISettingsService, SettingsService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ITfsService, TfsService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IDocumentsService, DocumentsService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ITelemetryService, TelemetryService>(new ContainerControlledLifetimeManager());
            _container.RegisterInstance<IFieldFactoryService>(new FieldFactoryService(t => _container.Resolve(t) as IField));
            _container.RegisterInstance<IAutomationFactoryService>(
                new AutomationFactoryService(t => _container.Resolve(t) as IAutomation));
            _container.RegisterInstance(messageQueue, new ExternallyControlledLifetimeManager());
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

        private static async void InitializeTelemetryService()
        {
            var telemetryService = _container.Resolve<ITelemetryService>();
            var response = await telemetryService.Initialize();
            if (!response.Success)
            {
                await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message), MainWindow.RootDialog);
            }
        }
    }
}
