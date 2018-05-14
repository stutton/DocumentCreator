using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Services.Settings;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.ViewModels.Navigation;
using Stutton.DocumentCreator.ViewModels.Pages;
using Unity;
using Unity.Lifetime;

namespace Stutton.DocumentCreator
{
    public static class Setup
    {
        public static void Configure(IUnityContainer container)
        {
            container.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISettingsService, SettingsService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITfsService, TfsService>(new ContainerControlledLifetimeManager());
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
                }
            }
        }

        private static IEnumerable<Type> GetInheritingTypes<T>(this Assembly assembly)
        {
            var types = assembly.DefinedTypes.Where(
                p => p.DeclaredConstructors.Any(
                         q => q.IsPublic) && typeof(T).GetTypeInfo().IsAssignableFrom(p) &&
                     !p.IsAbstract);
            return types.ToList();
        }
    }
}
