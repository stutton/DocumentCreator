using System;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Document;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.Template;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Automations.AttachToWorkItem
{
    public class AttachToWorkItemAutomationModel : AutomationModelBase
    {
        private readonly ITfsService _tfsService;

        public AttachToWorkItemAutomationModel(ITfsService tfsService)
        {
            _tfsService = tfsService;
        }

        public override string TypeDisplayName => "Attach to work item";
        public override string Description => "Upload and attache the document to work item";

        private string _name;
        public override string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public override async Task<IResponse> Execute(DocumentModel document, IWorkItem workItem, string documentPath)
        {
            var response = await _tfsService.AttachFileToWorkItemAsync(documentPath, workItem.Id);
            return !response.Success ? Response.FromFailure(response.Message) : Response.FromSuccess();
        }
    }
}
