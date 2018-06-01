using System;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Automations.AttachToWorkItem
{
    public class AttachToWorkItemAutomationModel : IAutomation
    {
        private readonly ITfsService _tfsService;
        public string Name => "Attach to work item";
        public string Description => "Upload and attache the document to work item";

        public AttachToWorkItemAutomationModel(ITfsService tfsService)
        {
            _tfsService = tfsService;
        }

        public Task<IResponse> Execute()
        {
            throw new NotImplementedException();
        }
    }
}
