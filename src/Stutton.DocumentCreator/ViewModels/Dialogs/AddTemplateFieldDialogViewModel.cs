using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Documents.Fields;
using Stutton.DocumentCreator.Services.Fields;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.Dialogs
{
    public class AddTemplateFieldDialogViewModel : Observable
    {
        private readonly IFieldFactoryService _fieldFactoryService;

        public ObservableDictionary<string, Type> AvailableFields
        {
            get => _availableFields;
            private set => Set(ref _availableFields, value);
        }

        private KeyValuePair<string, Type> _selectedFieldType;

        public KeyValuePair<string, Type> SelectedFieldType
        {
            get => _selectedFieldType;
            set => Set(ref _selectedFieldType, value);
        }

        private string _name;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _textToReplace;

        public string TextToReplace
        {
            get => _textToReplace;
            set => Set(ref _textToReplace, value);
        }

        private IField _currentField;

        public IField CurrentField
        {
            get => _currentField;
            set => Set(ref _currentField, value);
        }
        private ObservableDictionary<string, Type> _availableFields;



        public AddTemplateFieldDialogViewModel(IFieldFactoryService fieldFactoryService)
        {
            _fieldFactoryService = fieldFactoryService;
        }

        public async Task InitializeAsync()
        {
            var response = _fieldFactoryService.GetAllFieldKeys();
            if (!response.Success)
            {
                await DialogHost.Show(new ErrorMessageDialogViewModel(response.Message), MainWindow.RootDialog);
                return;
            }
            AvailableFields = new ObservableDictionary<string, Type>(response.Value);
        }

        protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedFieldType))
            {
                var response = _fieldFactoryService.CreateField(SelectedFieldType.Value);
                if (!response.Success)
                {
                    await DialogHost.Show(response.Message, MainWindow.RootDialog);
                    return;
                }

                var newField = response.Value;
                CurrentField = newField;
            }
        }
    }
}
