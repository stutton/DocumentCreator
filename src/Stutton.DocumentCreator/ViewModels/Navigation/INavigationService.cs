using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stutton.DocumentCreator.ViewModels.Pages;

namespace Stutton.DocumentCreator.ViewModels.Navigation
{
    public interface INavigationService
    {
        event EventHandler<NavigatingToPageEventArgs> NavigatingToPage;
        IPage CurrentPage { get; }
        IEnumerable<IPage> Pages { get; }
        void AddSidebarPage(string pageKey, IPage page);
        void AddOnDemandPage(string pageKey, Func<IPage> pageFactory);
        Task GoBack(object parameter = null);
        Task NavigateTo(string pageKey, object parameter = null);
    }
}
