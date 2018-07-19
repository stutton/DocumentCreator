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

namespace Stutton.DocumentCreator.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for MaterialOpenFolderDialog.xaml
    /// </summary>
    public partial class MaterialOpenFolderDialog : UserControl
    {
        public MaterialOpenFolderDialog()
        {
            InitializeComponent();
        }

        private void OpenDirectoryControl_OnDirectorySelected(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(true, this);
        }

        private void FileSystemControl_OnCancel(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(false, this);
        }
    }
}
