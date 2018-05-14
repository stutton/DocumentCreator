using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Navigation;
using Stutton.DocumentCreator.ViewModels.Pages;

namespace Stutton.DocumentCreator.ViewModels
{
    public class ShellViewModel : Observable
    {
        public ShellViewModel(INavigationService navigationService)
        {
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
            await Navigator.NavigateTo(WorkItemsPageViewModel.Key);
        }
    }
}
