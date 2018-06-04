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
using Stutton.DocumentCreator.ViewModels.Documents.Steps;
using Stutton.DocumentCreator.ViewModels.Pages;

namespace Stutton.DocumentCreator.Views.Pages
{
    /// <summary>
    /// Interaction logic for DocumentCreatorPage.xaml
    /// </summary>
    public partial class DocumentCreatorPage : UserControl
    {
        public DocumentCreatorPage()
        {
            InitializeComponent();
        }

        private void DocumentStepper_OnContinueNavigation(object sender, RoutedEventArgs e)
        {
            if (DocumentStepper.ActiveStep.Content is SummaryStepViewModel)
            {
                if (DataContext is DocumentCreatorPageViewModel vm)
                {
                    if (vm.FinishCommand.CanExecute(null))
                    {
                        vm.FinishCommand.Execute(null);
                    }
                }
            }
        }
    }
}
