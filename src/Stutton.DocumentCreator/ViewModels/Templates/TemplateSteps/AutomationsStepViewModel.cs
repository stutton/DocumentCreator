using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Documents.Automations;
using Stutton.DocumentCreator.Services.Automations;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.ViewModels.Templates.TemplateSteps
{
    public class AutomationsStepViewModel : Observable
    {
        private readonly IAutomationFactoryService _automationFactoryService;
        public ObservableCollection<IAutomation> Automations { get; }

        #region ICommand AddAutomationCommand

        private ICommand _addAutomationCommand;
        public ICommand AddAutomationCommand => _addAutomationCommand ?? (_addAutomationCommand = new RelayCommand(async () => await AddAutomation()));

        private async Task AddAutomation()
        {
            var dialogVm = new AddTemplateAutomationDialogViewModel(_automationFactoryService);
            await dialogVm.InitializeAsync();
            if ((bool) await DialogHost.Show(dialogVm, MainWindow.RootDialog))
            {
                Automations.Add(dialogVm.CurrentAutomation);
            }
        }

        #endregion

        #region ICommand DeleteAutomationCommand

        private ICommand _deleteAutomationCommand;
        public ICommand DeleteAutomationCommand => _deleteAutomationCommand ?? (_deleteAutomationCommand = new RelayCommand<IAutomation>(DeleteAutomation));

        private void DeleteAutomation(IAutomation automation)
        {
            Automations.Remove(automation);
        }

        #endregion

        public AutomationsStepViewModel(ObservableCollection<IAutomation> automations, IAutomationFactoryService automationFactoryService)
        {
            _automationFactoryService = automationFactoryService;
            Automations = automations;
        }
    }
}
