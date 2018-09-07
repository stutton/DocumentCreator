using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Document;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Vsts;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Automations.SetWorkItemField
{
    public class SetWorkItemFieldAutomationModel : AutomationModelBase, IRequiresInitialization
    {
        private readonly IVstsService _vstsService;

        public SetWorkItemFieldAutomationModel(IVstsService vstsService)
        {
            _vstsService = vstsService ?? throw new ArgumentNullException(nameof(vstsService));
        }

        public override string TypeDisplayName => "Set work item field";

        private string _name;
        public override string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public override string Description => "Set the value of a work item field to the given value";

        private ObservableCollection<WorkItemFieldModel> _workItemFields;
        public ObservableCollection<WorkItemFieldModel> WorkItemFields
        {
            get => _workItemFields;
            set => Set(ref _workItemFields, value);
        }

        private string _selectedField;
        public string SelectedField
        {
            get => _selectedField;
            set => Set(ref _selectedField, value);
        }

        private string _newFieldValue;
        public string NewFieldValue
        {
            get => _newFieldValue;
            set => Set(ref _newFieldValue, value);
        }

        public async Task<IResponse> Initialize()
        {
            var response = await _vstsService.GetWorkItemFields();
            if (!response.Success)
            {
                return response;
            }

            WorkItemFields = new ObservableCollection<WorkItemFieldModel>(response.Value);
            return Response.FromSuccess();
        }

        public override async Task<IResponse> Execute(DocumentModel document, IWorkItem workItem, string documentPath)
        {
            return await _vstsService.UpdateWorkItemAsync(workItem.Id, SelectedField, NewFieldValue);
        }
    }
}
