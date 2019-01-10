using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Fields;
using Stutton.DocumentCreator.Services.Fields;
using Stutton.DocumentCreator.Services.Telemetry;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.ViewModels.Templates.TemplateSteps
{
    public class FieldsStepViewModel : Observable
    {
        private readonly IFieldTemplateFactoryService _fieldFactoryService;
        private readonly ITelemetryService _telemetryService;

        public FieldsStepViewModel(ObservableCollection<FieldTemplateModelBase> fields, IFieldTemplateFactoryService fieldFactoryService, ITelemetryService telemetryService)
        {
            _fieldFactoryService = fieldFactoryService ?? throw new ArgumentNullException(nameof(fieldFactoryService));
            _telemetryService = telemetryService ?? throw new ArgumentNullException(nameof(telemetryService));
            Fields = fields ?? throw new ArgumentNullException(nameof(fields));
            foreach (var field in Fields)
            {
                field.RequestDeleteMe += HandleFieldRequestDeleteMe;
            }
        }

        public ObservableCollection<FieldTemplateModelBase> Fields { get; }

        private ObservableDictionary<string, Type> _availableFieldTypes;
        public ObservableDictionary<string, Type> AvailableFieldTypes
        {
            get => _availableFieldTypes;
            set => Set(ref _availableFieldTypes, value);
        }

        private Type _selectedType;

        public Type SelectedType
        {
            get => _selectedType;
            set
            {
                if (Set(ref _selectedType, value))
                {
                    if (AddFieldCommand is RelayCommand cmd)
                    {
                        cmd.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        #region ICommand AddFieldCommand

        private ICommand _addFieldCommand;

        public ICommand AddFieldCommand =>
            _addFieldCommand ?? (_addFieldCommand = new RelayCommand(async () => await AddField(), () => SelectedType != null));

        private async Task AddField()
        {
            if (SelectedType == null)
            {
                return;
            }
            var response = await _fieldFactoryService.CreateField(SelectedType);
            if (!response.Success)
            {
                await DialogHost.Show(response.Message, MainWindow.RootDialog);
                return;
            }

            response.Value.RequestDeleteMe += HandleFieldRequestDeleteMe;
            Fields.Add(response.Value);
            response.Value.IsExpanded = true;
        }

        #endregion

        public async Task InitializeAsync()
        {
            var response = _fieldFactoryService.GetAllFieldKeys();
            if (!response.Success)
            {
                _telemetryService.TrackFailedResponse(response);
                await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message, _telemetryService.SessionId), MainWindow.RootDialog);
                return;
            }
            AvailableFieldTypes = new ObservableDictionary<string, Type>(response.Value);
        }

        private void HandleFieldRequestDeleteMe(object sender, FieldTemplateModelBase field)
        {
            field.RequestDeleteMe -= HandleFieldRequestDeleteMe;
            Fields.Remove(field);
        }
    }
}