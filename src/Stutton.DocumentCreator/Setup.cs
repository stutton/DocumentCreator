using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Documents.Fields;
using Stutton.DocumentCreator.Services.Documents;
using Stutton.DocumentCreator.Services.Fields;
using Stutton.DocumentCreator.Services.Settings;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Navigation;
using Stutton.DocumentCreator.ViewModels.Pages;
using Unity;
using Unity.Lifetime;

namespace Stutton.DocumentCreator
{
    public static class Setup
    {
        public static void Configure(IUnityContainer container, ISnackbarMessageQueue messageQueue)
        {
            container.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISettingsService, SettingsService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITfsService, TfsService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDocumentsService, DocumentsService>(new ContainerControlledLifetimeManager());
            container.RegisterInstance<IFieldFactoryService>(new FieldFactoryService(t => container.Resolve(t) as IField));
            container.RegisterInstance(messageQueue, new ExternallyControlledLifetimeManager());
        }

        public static void LoadPages(IUnityContainer container)
        {
            var navigationService = container.Resolve<INavigationService>();
            var pageVmTypes = typeof(MainWindow).Assembly.GetInheritingTypes<IPage>();
            foreach (var pageVmType in pageVmTypes)
            {
                try
                {
                    if (!(container.Resolve(pageVmType) is IPage pageVm))
                    {
                        throw new InvalidOperationException(
                            $"Page type {pageVmType.Name} is not a valid IPage and will be ignored");
                    }

                    if (pageVm.IsOnDemandPage)
                        navigationService.AddOnDemandPage(pageVm.PageKey, () => container.Resolve(pageVmType) as IPage);
                    else
                        navigationService.AddSidebarPage(pageVm.PageKey, pageVm);
                }
                catch (InvalidOperationException ex)
                {
                    //_logger.Exception(ex, LogLevel.Warn);
                    Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
