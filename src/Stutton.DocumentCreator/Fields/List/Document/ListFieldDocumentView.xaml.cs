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

namespace Stutton.DocumentCreator.Fields.List.Document
{
    /// <summary>
    /// Interaction logic for ListFieldDocumentView.xaml
    /// </summary>
    public partial class ListFieldDocumentView : UserControl
    {
        public ListFieldDocumentView()
        {
            InitializeComponent();
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus((FrameworkElement) sender);
        }
    }
}
