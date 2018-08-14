using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using Stutton.DocumentCreator.Fields.WorkItemField.Document;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.WorkItemField.Template
{
    public class WorkItemFieldTemplateModel : FieldTemplateModelBase, IRequiresInitialization
    {
        private readonly ITfsService _tfsService;
        public const string Key = "WorkItemField";

        public override Type DtoType => typeof(WorkItemFieldTemplateDto);
        public override string Description => $"Replace part of the document with the value of the selected work item field";
        public override string TypeDisplayName => "Work Item Field";
        public override string FieldKey => Key;

        private ObservableCollection<WorkItemFieldModel> _workItemFields;
        public ObservableCollection<WorkItemFieldModel> WorkItemFields
        {
            get => _workItemFields;
            set => Set(ref _workItemFields, value);
        }

        private string _textToReplace;
        public string TextToReplace
        {
            get => _textToReplace;
            set => Set(ref _textToReplace, value);
        }

        private string _selectedField;
        public string SelectedField
        {
            get => _selectedField;
            set => Set(ref _selectedField, value);
        }

        private string _name;
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

            WorkItemFields = new ObservableCollection<WorkItemFieldModel>(tfsServiceResponse.Value);
            return Response.FromSuccess();
        }

        public override FieldDocumentModelBase GetDocumentField()
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
