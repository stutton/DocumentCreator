using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Stutton.DocumentCreator.Models.Documents.Fields;
using Stutton.DocumentCreator.Services.Fields;
using Stutton.DocumentCreator.Shared;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.ViewModels.Templates.TemplateSteps
{
    public class FieldsStepViewModel : Observable
    {
        private readonly IFieldFactoryService _fieldFactoryService;

        public ObservableCollection<IField> Fields { get; }

        public ObservableCollection<string> AvailableFieldTypes
        {
            get => _availableFieldTypes;
            set => Set(ref _availableFieldTypes, value);
        }

        #region ICommand AddFieldCommand

        private ICommand _addFieldCommand;
        public ICommand AddFieldCommand => _addFieldCommand ?? (_addFieldCommand = new RelayCommand(async () => await AddField()));

        private async Task AddField()
        {
            var dialogVm = new AddTemplateFieldDialogViewModel(_fieldFactoryService);
            await dialogVm.InitializeAsync();
            if ((bool) await DialogHost.Show(dialogVm, MainWindow.RootDialog))
            {
                Fields.Add(dialogVm.CurrentField);
            }
        }

        #endregion

        #region ICommand DeleteFieldCommand

        private ICommand _deleteFieldCommand;
        private ObservableCollection<string> _availableFieldTypes;
        public ICommand DeleteFieldCommand => _deleteFieldCommand ?? (_deleteFieldCommand = new RelayCommand<IField>(DeleteField));

        private void DeleteField(IField field)
        {
            Fields.Remove(field);
        }

        #endregion
        
        public FieldsStepViewModel(ObservableCollection<IField> fields, IFieldFactoryService fieldFactoryService)
        {
            _fieldFactoryService = fieldFactoryService;
            Fields = fields;
        }
    }
}
