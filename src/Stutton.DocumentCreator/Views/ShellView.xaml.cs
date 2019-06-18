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
using System.Windows.Media.Animation;
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
            DataContextChanged += ShellView_DataContextChanged;
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

        private void ShellView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_vm == null)
            {
                _vm = DataContext as ShellViewModel;
            }

            if (_vm == null)
            {
                return;
            }

            if (TryFindResource("PrimaryHueMidBrush") is SolidColorBrush primaryMid)
            {
                var unfrozenBrush = new SolidColorBrush(primaryMid.Color);
                TitleColorZone.Background = unfrozenBrush;
            }
            _vm.Navigator.NavigatingToPage += Navigator_NavigatingToPage;
        }

        private void Navigator_NavigatingToPage(object sender, NavigatingToPageEventArgs args)
        {
            if(args.OldPage != null)
            {
                args.OldPage.PropertyChanged -= CurrentPage_PropertyChanged;
            }
            if (args.NewPage != null)
            {
                args.NewPage.PropertyChanged += CurrentPage_PropertyChanged;
            }
            if(args.OldPage != null && args.NewPage != null && args.OldPage.IsInEditMode != args.NewPage.IsInEditMode)
            {
                AnimateTitleColorZoneBackground();
            }
        }

        private void CurrentPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName != nameof(IPage.IsInEditMode))
            {
                return;
            }

            AnimateTitleColorZoneBackground();
        }

        private void AnimateTitleColorZoneBackground()
        {
            if (_vm == null)
            {
                _vm = DataContext as ShellViewModel;
            }

            if (_vm == null)
            {
                return;
            }

            var isInEditMode = _vm.Navigator.CurrentPage.IsInEditMode;
            var toColor = isInEditMode ? TryFindResource("MaterialDesignPaper") as SolidColorBrush
                                       : TryFindResource("PrimaryHueMidBrush") as SolidColorBrush;
            var animation = new ColorAnimation
            {
                FillBehavior = FillBehavior.HoldEnd,
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                To = toColor.Color
            };

            TitleColorZone.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }
    }
}
