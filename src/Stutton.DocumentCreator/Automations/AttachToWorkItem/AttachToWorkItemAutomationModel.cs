using System;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Document;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.Template;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Services.Vsts;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Automations.AttachToWorkItem
{
    public class AttachToWorkItemAutomationModel : AutomationModelBase
    {
        private readonly IVstsService _vstsService;

        public AttachToWorkItemAutomationModel(IVstsService vstsService)
        {
            _vstsService = vstsService ?? throw new ArgumentNullException(nameof(vstsService));
        }

        public override string TypeDisplayName => "Attach to work item";
        public override string Description => "Upload and attach the document to work item";

        private string _name;
        public override string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public override async Task<IResponse> Execute(DocumentModel document, IWorkItem workItem, string documentPath)
        {
            var response = await _vstsService.AttachFileToWorkItemAsync(documentPath, workItem.Id);
            return !response.Success ? Response.FromFailure(response.Message) : Response.FromSuccess();
        }
    }
}
