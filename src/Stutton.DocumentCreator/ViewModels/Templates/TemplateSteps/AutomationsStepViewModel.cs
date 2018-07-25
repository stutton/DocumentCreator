using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Automations;
using Stutton.DocumentCreator.Services.Automations;
using Stutton.DocumentCreator.Services.Telemetry;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.ViewModels.Templates.TemplateSteps
{
    public class AutomationsStepViewModel : Observable
    {
        private readonly IAutomationFactoryService _automationFactoryService;
        private readonly ITelemetryService _telemetryService;

        public ObservableCollection<IAutomation> Automations { get; }

        private ObservableDictionary<string, Type> _availableAutomationTypes;
        public ObservableDictionary<string, Type> AvailableAutomationTypes
        {
            get => _availableAutomationTypes;
            set => Set(ref _availableAutomationTypes, value);
        }

        private Type _selectedType;
        public Type SelectedType
        {
            get => _selectedType;
            set
            {
                if (Set(ref _selectedType, value))
                {
                    if (AddAutomationCommand is RelayCommand cmd)
                    {
                        cmd.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        #region ICommand AddAutomationCommand

        private ICommand _addAutomationCommand;
        public ICommand AddAutomationCommand => _addAutomationCommand ?? (_addAutomationCommand = new RelayCommand(async () => await AddAutomation()));

        private async Task AddAutomation()
        {
            if (SelectedType == null)
            {
                return;
            }

            var response = _automationFactoryService.CreateAutomation(SelectedType);
            if (!response.Success)
            {
                _telemetryService.TrackFailedResponse(response);
                await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message), MainWindow.RootDialog);
                return;
            }

            var newAutomation = response.Value;
            newAutomation.RequestDeleteMe += AutomationOnRequestDeleteMe;
            Automations.Add(response.Value);
        }

        #endregion

        public AutomationsStepViewModel(ObservableCollection<IAutomation> automations, IAutomationFactoryService automationFactoryService, ITelemetryService telemetryService)
        {
            _automationFactoryService = automationFactoryService;
            _telemetryService = telemetryService;
            Automations = automations;
            foreach (var automation in Automations)
            {
                automation.RequestDeleteMe += AutomationOnRequestDeleteMe;
            }
        }

        public async Task InitializeAsync()
        {
            var response = _automationFactoryService.GetAllAutomationKeys();
            if (!response.Success)
            {
                _telemetryService.TrackFailedResponse(response);
                await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message), MainWindow.RootDialog);
                return;
            }
            AvailableAutomationTypes = new ObservableDictionary<string, Type>(response.Value);
        }

        private void AutomationOnRequestDeleteMe(object sender, EventArgs e)
        {
            var automation = (IAutomation) sender;
            automation.RequestDeleteMe -= AutomationOnRequestDeleteMe;
            Automations.Remove(automation);
        }
    }
}
