using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Documents
{
    public interface IDocumentsService
    {
        Task<IResponse<IEnumerable<DocumentModel>>> GetDocuments();
        Task<IResponse> SaveDocumentTemplate(DocumentModel document);
    }
}
