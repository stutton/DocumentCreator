using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Packaging;
using Newtonsoft.Json;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Models.Document;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.Template;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;
using Directory = System.IO.Directory;

namespace Stutton.DocumentCreator.Services.Document
{
    public class DocumentService : IDocumentService
    {
        private readonly IServiceResolver _serviceResolver;
        private readonly IMapper _mapper;
        private static readonly string TempDirectory = Path.GetTempPath();

        private readonly string _documentSaveDirectoryName =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\DocumentCreator\\Saves";

        private readonly string _documentSaveFileExtension = "dcs";

        public DocumentService(IServiceResolver serviceResolver, IMapper mapper)
        {
            _serviceResolver = serviceResolver;
            _mapper = mapper;
        }

        public async Task<IResponse<string>> CreateDocumentAsync(DocumentModel model, IWorkItem workItem)
        {
            if (!File.Exists(model.Details.TemplateFilePath))
            {
                return Response<string>.FromFailure($"Template file not found '{model.Details.TemplateFilePath}'");
            }

            //TODO: Create meaningful file name

            var outFile = Path.Combine(Path.GetTempPath(), model.Details.GeneratedFileName);
            //var outFile = Path.GetTempFileName();
            await Task.Run(() => File.Copy(model.Details.TemplateFilePath, outFile, true));
            using (var doc = WordprocessingDocument.Open(outFile, true))
            {
                foreach (var field in model.Fields)
                {
                    var response = await field.ModifyDocument(doc, workItem);
                    if (!response.Success)
                    {
                        // TODO: Should we fail if a field does?
                        return Response<string>.FromFailure(response.Message);
                    }
                }
            }

            return Response<string>.FromSuccess(outFile);
        }

        public async Task<IResponse> SaveDocumentAsync(DocumentModel model, IWorkItem workItem, string name)
        {
            try
            {
                if (!Directory.Exists(_documentSaveDirectoryName))
                {
                    Directory.CreateDirectory(_documentSaveDirectoryName);
                }

                model.FileName = name;

                var documentDto = _mapper.Map<DocumentDto>(model);

                var documentJson = await Task.Run(
                    () => JsonConvert.SerializeObject(
                        documentDto,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Objects,
                            TypeNameAssemblyFormatHandling =
                                TypeNameAssemblyFormatHandling.Simple
                        }));

                await Task.Run(() => File.WriteAllText(
                                   $"{_documentSaveDirectoryName}\\{name}.{_documentSaveFileExtension}",
                                   documentJson));
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException($"Failed to save document {model.Details.Name}", ex);
            }
        }

        public async Task<IResponse<IEnumerable<DocumentModel>>> LoadAllSavedDocumentsAsync()
        {
            try
            {
                if (!Directory.Exists(_documentSaveDirectoryName))
                {
                    return Response<IEnumerable<DocumentModel>>.FromSuccess(new List<DocumentModel>());
                }

                var documentFiles =
                    await Task.Run(
                        () => Directory.GetFiles(_documentSaveDirectoryName, $"*.{_documentSaveFileExtension}"));

                if (!documentFiles.Any())
                {
                    return Response<IEnumerable<DocumentModel>>.FromSuccess(new List<DocumentModel>());
                }

                var documents = new List<DocumentModel>();
                foreach (var documentFile in documentFiles)
                {
                    var documentJson = await Task.Run(() => File.ReadAllText(documentFile));
                    var documentDto = await Task.Run(
                        () => JsonConvert.DeserializeObject<DocumentDto>(
                            documentJson,
                            new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Objects
                            }));
                    var document = _mapper.Map<DocumentModel>(documentDto);
                    documents.Add(document);
                }

                return Response<IEnumerable<DocumentModel>>.FromSuccess(documents);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<DocumentModel>>.FromException("Failed to load saved documents", ex);
            }
        }

        public async Task<IResponse> DeleteSavedDocumentAsync(DocumentModel model)
        {
            try
            {
                if (!Directory.Exists(_documentSaveDirectoryName))
                {
                    return Response.FromFailure("Document directory does not exist", ResponseCode.FileNotFound);
                }

                var fileToDelete = $"{_documentSaveDirectoryName}\\{model.FileName}.{_documentSaveFileExtension}";

                if (!File.Exists(fileToDelete))
                {
                    return Response.FromFailure("Document save file does not exist", ResponseCode.FileNotFound);
                }

                await Task.Run(() => File.Delete(fileToDelete));

                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException("Error deleting document", ex);
            }
        }

        public async Task<IResponse> ExecuteAutomationsAsync(DocumentModel model, IWorkItem workItem, string documentPath)
        {
            foreach (var automation in model.Automations)
            {
                var response = await automation.Execute(model, workItem, documentPath);
                if (!response.Success)
                {
                    return response;
                }
            }

            return Response.FromSuccess();
        }
    }
}
