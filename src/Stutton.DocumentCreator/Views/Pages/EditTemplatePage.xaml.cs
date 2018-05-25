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
using Stutton.DocumentCreator.ViewModels.Pages;
using Stutton.DocumentCreator.ViewModels.Templates.TemplateSteps;

namespace Stutton.DocumentCreator.Views.Pages
{
    /// <summary>
    /// Interaction logic for EditTemplatePage.xaml
    /// </summary>
    public partial class EditTemplatePage : UserControl
    {
        public EditTemplatePage()
        {
            InitializeComponent();
        }

        private void Stepper_OnContinueNavigation(object sender, RoutedEventArgs e)
        {
            if (TemplateStepper.ActiveStep.Content is SummaryStepViewModel)
            {
                if (DataContext is EditTemplatePageViewModel vm)
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
