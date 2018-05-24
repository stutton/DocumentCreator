using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Documents
{
    public class DocumentsService : IDocumentsService
    {
        private readonly string _documentTemplatesDirectoryName =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\DocumentCreator\\Templates";
        public IResponse<IEnumerable<DocumentModel>> GetDocuments()
        {
            try
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
            catch (Exception ex)
            {
                return Response<IEnumerable<DocumentModel>>.FromException("Failed to get saved documents", ex);
            }
        }

        public async Task<IResponse> SaveDocumentTemplate(DocumentModel document)
        {
            try
            {
                if (!Directory.Exists(_documentTemplatesDirectoryName))
                {
                    Directory.CreateDirectory(_documentTemplatesDirectoryName);
                }

                var documentJson = await Task.Run(() => JsonConvert.SerializeObject(document));
                await Task.Run(() => File.WriteAllText($"{_documentTemplatesDirectoryName}\\{DateTime.Now:yyyyMMddHHmmss}.template", documentJson));
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException($"Failed to save document {document.Details.Name}", ex);
            }
        }
    }
}
