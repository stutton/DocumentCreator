using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Navigation;
using Stutton.DocumentCreator.ViewModels.Toolbar;

namespace Stutton.DocumentCreator.ViewModels.Pages
{
    public abstract class PageBase : Observable, IPage
    {
        private readonly INavigationService _navigator;
        private bool _isFullscreen;
        private bool _isInEditMode;
        public abstract string PageKey { get; }
        public abstract string Title { get; }
        public abstract bool IsOnDemandPage { get; }

        public virtual int PageOrder => 0;

        protected PageBase(INavigationService navigator)
        {
            _navigator = navigator;
        }

        public bool IsInEditMode
        {
            get => _isInEditMode;
            protected set => Set(ref _isInEditMode, value);
        }

        public bool IsFullscreen
        {
            get => _isFullscreen;
            protected set => Set(ref _isFullscreen, value);
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            protected set => Set(ref _isBusy, value);
        }

        #region Back Command

        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ?? (_backCommand = new RelayCommand(async () => await Back()));

        private async Task Back()
        {
            await _navigator.GoBack();
        }

        #endregion

        public ToolbarOptions ToolBar { get; } = new ToolbarOptions();

        public virtual Task NavigatedTo(object parameter)
        {
            return Task.FromResult(true);
        }
    }
}
