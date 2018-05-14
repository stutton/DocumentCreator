using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Properties;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Pages;

namespace Stutton.DocumentCreator.ViewModels.Navigation
{
    public class NavigationService : Observable, INavigationService
    {
        private readonly Stack<IPage> _history = new Stack<IPage>();
        private readonly Dictionary<string, IPage> _singletonPages = new Dictionary<string, IPage>();
        private readonly Dictionary<string, Func<IPage>> _onDemandPages = new Dictionary<string, Func<IPage>>();
        private IPage _currentPage;

        public IPage CurrentPage
        {
            get => _currentPage;
            set => Set(ref _currentPage, value);
        }

        public IEnumerable<IPage> Pages => _singletonPages.Values.OrderBy(p => p.PageOrder);

        public void AddSidebarPage(string pageKey, IPage page)
        {
            _singletonPages.Add(pageKey, page);
        }

        public void AddOnDemandPage(string pageKey, Func<IPage> pageFactor)
        {
            _onDemandPages.Add(pageKey, pageFactor);
        }

        public async Task GoBack(object parameter = null)
        {
            var page = _history.Pop();
            CurrentPage = page;
            await CurrentPage.NavigatedTo(parameter);
        }

        public async Task NavigateTo(string pageKey, object parameter = null)
        {
            if (!_singletonPages.ContainsKey(pageKey) && !_onDemandPages.ContainsKey(pageKey))
            {
                throw new ArgumentException(string.Format(Resources.NavigationService_NavigateTo_No_such_page, pageKey), nameof(pageKey));
            }

            if (CurrentPage != null && pageKey == CurrentPage.PageKey)
            {
                return;
            }

            IPage page;
            if (_singletonPages.ContainsKey(pageKey))
            {
                page = _singletonPages[pageKey];
                _history.Clear();
            }
            else
            {
                page = _onDemandPages[pageKey].Invoke();
                if (CurrentPage != null)
                {
                    _history.Push(CurrentPage);
                }
            }

            CurrentPage = page;
            await CurrentPage.NavigatedTo(parameter);
        }
    }
}
