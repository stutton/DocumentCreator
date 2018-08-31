using System.Collections.Generic;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Template;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Templates
{
    public interface ITemplatesService
    {
        Task<IResponse<IEnumerable<DocumentTemplateModel>>> GetDocuments();
        Task<IResponse> SaveDocumentTemplate(DocumentTemplateModel document);
        Task<IResponse> DeleteDocumentTemplate(DocumentTemplateModel document);
        Task<IResponse> ShareDocumentTemplate(DocumentTemplateModel document, string fileName);
        Task<IResponse> ImportDocumentTemplate(string fileName);
    }
}
