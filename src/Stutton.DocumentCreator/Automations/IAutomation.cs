using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.Template;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Automations
{
    public interface IAutomation
    {
        string Name { get; }

        string Description { get; }

        Task<IResponse> Execute(DocumentTemplateModel model, IWorkItem workItem, string documentPath, IServiceResolver resolver);
    }
}