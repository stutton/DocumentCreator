using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using Stutton.DocumentCreator.Fields.WorkItemField.Document;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.WorkItemField.Template
{
    [DataContract(Name = "WorkItemField")]
    public class WorkItemFieldTemplateModel : FieldTemplateBase, IRequiresInitialization
    {
        [IgnoreDataMember]
        private readonly ITfsService _tfsService;
        public const string Key = "WorkItemField";

        [IgnoreDataMember]
        public override string Description => $"Replace '{TextToReplace}' with the value of '{SelectedField}' from the selected work item";
        [IgnoreDataMember]
        public override string TypeDisplayName => "Work Item Field";
        [IgnoreDataMember]
        public override string FieldKey => Key;

        private ObservableCollection<string> _workItemFields;

        [IgnoreDataMember]
        public ObservableCollection<string> WorkItemFields
        {
            get => _workItemFields;
            set => Set(ref _workItemFields, value);
        }

        private string _textToReplace;
        [DataMember]
        public string TextToReplace
        {
            get => _textToReplace;
            set => Set(ref _textToReplace, value);
        }

        private string _selectedField;
        [DataMember]
        public string SelectedField
        {
            get => _selectedField;
            set => Set(ref _selectedField, value);
        }

        private string _name;
        [DataMember]
        public override string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public WorkItemFieldTemplateModel(ITfsService tfsService)
        {
            _tfsService = tfsService;
        }

        public async Task<IResponse> Initialize()
        {
            var tfsServiceResponse = await _tfsService.GetWorkItemFields();
            if (!tfsServiceResponse.Success)
            {
                return tfsServiceResponse;
            }

            WorkItemFields = new ObservableCollection<string>(tfsServiceResponse.Value);
            return Response.FromSuccess();
        }

        public override IFieldDocument GetDocumentField()
        {
            var documentField = new WorkItemFieldDocumentModel(_tfsService)
            {
                Name = Name,
                SelectedField = SelectedField,
                TextToReplace = TextToReplace
            };
            return documentField;
        }
    }
}
