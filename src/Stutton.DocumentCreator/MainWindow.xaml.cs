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

        private readonly IUnityContainer _container;
        private readonly ShellViewModel _shellViewModel;

        public MainWindow()
        {
            InitializeComponent();

            _container = new UnityContainer();

            Setup.Configure(_container, MainSnackbar.MessageQueue);
            Setup.LoadPages(_container);

            _shellViewModel = _container.Resolve<ShellViewModel>();
            ShellView.DataContext = _shellViewModel;
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            _container.Dispose();
        }

        private async void DialogHost_OnLoaded(object sender, RoutedEventArgs e)
        {
            ShellView.EnableNavigation();
            await _shellViewModel.LoadAsync();
        }
    }
}
