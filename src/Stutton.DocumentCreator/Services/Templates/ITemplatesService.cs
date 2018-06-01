using System.Collections.Generic;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Templates
{
    public interface ITemplatesService
    {
        Task<IResponse<IEnumerable<DocumentModel>>> GetDocuments();
        Task<IResponse> SaveDocumentTemplate(DocumentModel document);
    }
}
