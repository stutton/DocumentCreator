using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Templates
{
    public class TemplatesService : ITemplatesService
    {
        private readonly string _documentTemplatesDirectoryName =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\DocumentCreator\\Templates";
        private readonly string _documentTemplateFileExtension = "template";

        public async Task<IResponse<IEnumerable<DocumentModel>>> GetDocuments()
        {
            try
            {
                if (!Directory.Exists(_documentTemplatesDirectoryName))
                {
                    return Response<IEnumerable<DocumentModel>>.FromSuccess(new List<DocumentModel>());
                }

                var templateFiles = await Task.Run(() =>
                    Directory.GetFiles(_documentTemplatesDirectoryName, $"*.{_documentTemplateFileExtension}"));

                if (!templateFiles.Any())
                {
                    return Response<IEnumerable<DocumentModel>>.FromSuccess(new List<DocumentModel>());
                }

                var templates = new List<DocumentModel>();
                foreach (var templateFile in templateFiles)
                {
                    var templateJson = await Task.Run(() => File.ReadAllText(templateFile));
                    var template = await Task.Run(() => JsonConvert.DeserializeObject<DocumentModel>(templateJson, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects
                    }));
                    templates.Add(template);
                }

                return Response<IEnumerable<DocumentModel>>.FromSuccess(templates);
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

                var documentJson = await Task.Run(() => JsonConvert.SerializeObject(document, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                }));
                await Task.Run(() => File.WriteAllText($"{_documentTemplatesDirectoryName}\\{document.Id}.{_documentTemplateFileExtension}", documentJson));
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException($"Failed to save document {document.Details.Name}", ex);
            }
        }
    }
}
