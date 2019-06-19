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
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;
using Stutton.DocumentCreator.ViewModels.Navigation;
using Stutton.DocumentCreator.ViewModels.Pages;

namespace Stutton.DocumentCreator.ViewModels
{
    public class ShellViewModel : Observable
    {
        private readonly IWindow _mainWindow;

        public ShellViewModel(INavigationService navigationService, IWindow mainWindow)
        {
            Navigator = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _mainWindow = mainWindow;
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

        private ICommand _showAboutCommand;
        public ICommand ShowAboutCommand => _showAboutCommand ?? (_showAboutCommand = new RelayCommand(async () => await ShowAbout()));

        private static async Task ShowAbout() => await DialogHost.Show(new AboutDialogViewModel(), MainWindow.RootDialog);

        #region Close Command

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(Close));

        private void Close()
        {
            _mainWindow.Close();
        }

        #endregion

        #region ToggleMaxMin Command

        private ICommand _toggleMaxMinCommand;
        public ICommand ToggleMaxMinCommand => _toggleMaxMinCommand ?? (_toggleMaxMinCommand = new RelayCommand(ToggleMaxMin));

        private void ToggleMaxMin()
        {
            if (_mainWindow.IsMaximized)
            {
                _mainWindow.Restore();
                return;
            }
            _mainWindow.Maximize();
        }

        #endregion

        #region Minimize Command

        private ICommand _MinimizeCommand;
        public ICommand MinimizeCommand => _MinimizeCommand ?? (_MinimizeCommand = new RelayCommand(Minimize));

        private void Minimize()
        {
            _mainWindow.Minimize();
        }

        #endregion

        public async Task LoadAsync()
        {
            await Navigator.NavigateTo(DocumentsPageViewModel.Key);
        }
    }
}
