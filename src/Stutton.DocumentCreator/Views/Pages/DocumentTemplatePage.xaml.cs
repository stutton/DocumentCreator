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
using Stutton.DocumentCreator.ViewModels.Documents.DocumentTemplateSteps;
using Stutton.DocumentCreator.ViewModels.Pages;

namespace Stutton.DocumentCreator.Views.Pages
{
    /// <summary>
    /// Interaction logic for DocumentTemplatePage.xaml
    /// </summary>
    public partial class DocumentTemplatePage : UserControl
    {
        public DocumentTemplatePage()
        {
            InitializeComponent();
        }

        private void Stepper_OnContinueNavigation(object sender, RoutedEventArgs e)
        {
            if (TemplateStepper.ActiveStep.Content is SummaryStepViewModel)
            {
                if (DataContext is DocumentTemplatePageViewModel vm)
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
