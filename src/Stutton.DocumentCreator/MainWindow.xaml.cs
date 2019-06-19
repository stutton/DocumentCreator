using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.ViewModels;
using Unity;
using Mdx = MaterialDesignExtensions.Themes;

namespace Stutton.DocumentCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWindow
    {
        public const string RootDialog = "RootDialog";
        
        private ShellViewModel _shellViewModel;
        private Mdx.PaletteHelper _paletteHelper;

        public MainWindow()
        {
            InitializeComponent();

            _paletteHelper = new Mdx.PaletteHelper();
            var darkThemeEnabled = Properties.Settings.Default.DarkThemeEnabled;
            _paletteHelper.SetLightDark(darkThemeEnabled);
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            Setup.Dispose();
        }

        private async void DialogHost_OnLoaded(object sender, RoutedEventArgs e)
        {
            var debugging = false;
#if DEBUG
            debugging = true;
#endif
            await Setup.DoSetup(MainSnackbar.MessageQueue, debugging, this);

            _shellViewModel = Setup.GetShellViewModel();
            ShellView.DataContext = _shellViewModel;

            ShellView.EnableNavigation();
            await _shellViewModel.LoadAsync();
        }

        bool IWindow.IsMaximized => WindowState == WindowState.Maximized;
        void IWindow.Close() => Close();
        void IWindow.Maximize() => WindowState = WindowState.Maximized;
        void IWindow.Restore() => WindowState = WindowState.Normal;
        void IWindow.Minimize() => WindowState = WindowState.Minimized;
    }
}
