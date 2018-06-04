using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Document
{
    public interface IDocumentService
    {
        Task<IResponse<string>> CreateDocument(DocumentModel model);
        Task<IResponse> ExecuteAutomations(DocumentModel model, IWorkItem workItem, string documentPath);
    }
}
