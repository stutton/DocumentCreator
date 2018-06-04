using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Fields;
using Stutton.DocumentCreator.Services.Fields;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.ViewModels.Templates.TemplateSteps
{
    public class FieldsStepViewModel : Observable
    {
        private readonly IFieldFactoryService _fieldFactoryService;

        public FieldsStepViewModel(ObservableCollection<IField> fields, IFieldFactoryService fieldFactoryService)
        {
            _fieldFactoryService = fieldFactoryService;
            Fields = fields;
        }

        public ObservableCollection<IField> Fields { get; }

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
            Fields.Add(response.Value);
        }

        #endregion

        #region ICommand DeleteFieldCommand

        private ICommand _deleteFieldCommand;

        public ICommand DeleteFieldCommand =>
            _deleteFieldCommand ?? (_deleteFieldCommand = new RelayCommand<IField>(DeleteField));

        private void DeleteField(IField field)
        {
            Fields.Remove(field);
        }

        #endregion

        public async Task InitializeAsync()
        {
            var response = _fieldFactoryService.GetAllFieldKeys();
            if (!response.Success)
            {
                await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message), MainWindow.RootDialog);
                return;
            }
            AvailableFieldTypes = new ObservableDictionary<string, Type>(response.Value);
        }
    }
}