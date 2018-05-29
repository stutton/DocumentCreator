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

namespace Stutton.DocumentCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string RootDialog = "RootDialog";
        
        private ShellViewModel _shellViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            Setup.Dispose();
        }

        private async void DialogHost_OnLoaded(object sender, RoutedEventArgs e)
        {
            await Setup.DoSetup(MainSnackbar.MessageQueue);

            _shellViewModel = Setup.GetShellViewModel();
            ShellView.DataContext = _shellViewModel;

            ShellView.EnableNavigation();
            await _shellViewModel.LoadAsync();
        }
    }
}
