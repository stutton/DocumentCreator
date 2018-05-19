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
using Stutton.DocumentCreator.ViewModels.Navigation;
using Stutton.DocumentCreator.ViewModels.Pages;

namespace Stutton.DocumentCreator.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : UserControl
    {
        private ShellViewModel _vm;
        private bool _navigationEnabled = false;

        public ShellView()
        {
            InitializeComponent();
        }

        public void EnableNavigation()
        {
            _navigationEnabled = true;
        }

        private async void PagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_navigationEnabled)
            {
                return;
            }

            if (_vm == null)
            {
                _vm = DataContext as ShellViewModel;
            }
            var pageToNavigate = (IPage)PagesListBox.SelectedItem;
            if (_vm != null)
            {
                await _vm.Navigator.NavigateTo(pageToNavigate.PageKey).ConfigureAwait(true);
            }
        }
    }
}
