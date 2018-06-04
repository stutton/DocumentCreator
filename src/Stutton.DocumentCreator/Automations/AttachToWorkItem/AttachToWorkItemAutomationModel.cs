using System;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Automations.AttachToWorkItem
{
    public class AttachToWorkItemAutomationModel : IAutomation
    {
        public string Name => "Attach to work item";
        public string Description => "Upload and attache the document to work item";

        public async Task<IResponse> Execute(DocumentModel model, IWorkItem workItem, string documentPath, IServiceResolver serviceResolver)
        {
            var serviceResponse = serviceResolver.Resolve<ITfsService>();
            if (!serviceResponse.Success)
            {
                return Response.FromFailure(serviceResponse.Message);
            }

            var tfsService = serviceResponse.Value;
            var response = await tfsService.AttachFileToWorkItemAsync(documentPath, workItem.Id);
            if (!response.Success)
            {
                return Response.FromFailure(response.Message);
            }

            return Response.FromSuccess();
        }
    }
}
