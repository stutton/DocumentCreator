using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Documents.Automations;
using Stutton.DocumentCreator.Services.Automations;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.Dialogs
{
    public class AddTemplateAutomationDialogViewModel : Observable
    {
        private readonly IAutomationFactoryService _automationFactoryService;
        private ObservableDictionary<string, Type> _availableAutomations;

        private IAutomation _currentAutomation;

        private KeyValuePair<string, Type> _selectedAutomationType;

        public AddTemplateAutomationDialogViewModel(IAutomationFactoryService automationFactoryService)
        {
            _automationFactoryService = automationFactoryService;
        }

        public ObservableDictionary<string, Type> AvailableAutomations
        {
            get => _availableAutomations;
            private set => Set(ref _availableAutomations, value);
        }

        public KeyValuePair<string, Type> SelectedAutomationType
        {
            get => _selectedAutomationType;
            set => Set(ref _selectedAutomationType, value);
        }

        public IAutomation CurrentAutomation
        {
            get => _currentAutomation;
            set => Set(ref _currentAutomation, value);
        }

        public async Task InitializeAsync()
        {
            var response = _automationFactoryService.GetAllAutomationKeys();
            if (!response.Success)
            {
                await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message), MainWindow.RootDialog);
                return;
            }
            
            AvailableAutomations = new ObservableDictionary<string, Type>(response.Value);
        }

        protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedAutomationType))
            {
                var response = _automationFactoryService.CreateAutomation(SelectedAutomationType.Value);
                if (!response.Success)
                {
                    await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message), MainWindow.RootDialog);
                    return;
                }

                var newAutomation = response.Value;
                CurrentAutomation = newAutomation;
            }
        }
    }
}