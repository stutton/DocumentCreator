using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Documents
{
    public class DocumentsService : IDocumentsService
    {
        public IResponse<IEnumerable<DocumentModel>> GetDocuments()
        {
            var sampleDoc1 = new DocumentModel();
            sampleDoc1.Details.Name = "Sample Test Doc";
            sampleDoc1.Details.Description = "This will create a sample test doc.";
            sampleDoc1.Details.DocumentType = DocumentType.Word;

            var sampleDoc2 = new DocumentModel();
            sampleDoc2.Details.Name = "Sample Release Doc";
            sampleDoc2.Details.Description = "This will create a sample release doc.";
            sampleDoc2.Details.DocumentType = DocumentType.Word;

            return Response<IEnumerable<DocumentModel>>.FromSuccess(
                new List<DocumentModel>
                {
                    sampleDoc1,
                    sampleDoc2
                });
        }
    }
}
