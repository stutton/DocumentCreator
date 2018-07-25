using System;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Document;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.Template;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Automations
{
    public interface IAutomation
    {
        event EventHandler<EventArgs> RequestDeleteMe;

        string TypeDisplayName { get; }

        string Description { get; }

        string Name { get; set; }

        Task<IResponse> Execute(DocumentModel model, IWorkItem workItem, string documentPath);
    }
}