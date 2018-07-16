using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Document;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.Template;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Document
{
    public interface IDocumentService
    {
        Task<IResponse<string>> CreateDocumentAsync(DocumentModel model, IWorkItem workItem);
        Task<IResponse> SaveDocumentAsync(DocumentModel model, IWorkItem workItem, string name);
        Task<IResponse<IEnumerable<DocumentModel>>> LoadAllSavedDocumentsAsync();
        Task<IResponse> ExecuteAutomationsAsync(DocumentModel model, IWorkItem workItem, string documentPath);
    }
}
