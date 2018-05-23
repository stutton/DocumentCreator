using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.Documents.Automations
{
    public class AttachToWorkItemAutomation : IAutomation
    {
        private readonly ITfsService _tfsService;
        public string Name => "Attach to work item";
        public string Description => "Upload and attache the document to work item";

        public AttachToWorkItemAutomation(ITfsService tfsService)
        {
            _tfsService = tfsService;
        }

        public Task<IResponse> Execute()
        {
            throw new NotImplementedException();
        }
    }
}
