using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace Stutton.DocumentCreator.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for MaterialSaveFileDialog.xaml
    /// </summary>
    public partial class MaterialSaveFileDialog : UserControl
    {
        public MaterialSaveFileDialog()
        {
            InitializeComponent();
        }

        private void BaseFileControl_OnFileSelected(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(true, this);
        }

        private void FileSystemControl_OnCancel(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(false, this);
        }
    }
}
