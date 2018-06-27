using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Models.Document;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.Template;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Document
{
    public class DocumentService : IDocumentService
    {
        private readonly IServiceResolver _serviceResolver;
        private static readonly string TempDirectory = Path.GetTempPath();

        public DocumentService(IServiceResolver serviceResolver)
        {
            _serviceResolver = serviceResolver;
        }

        public async Task<IResponse<string>> CreateDocument(DocumentModel model, IWorkItem workItem)
        {
            if (!File.Exists(model.Details.TemplateFilePath))
            {
                return Response<string>.FromFailure($"Template file not found '{model.Details.TemplateFilePath}'");
            }

            //TODO: Create meaningful file name
            var outFile = Path.GetTempFileName();
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

        public async Task<IResponse> ExecuteAutomations(DocumentModel model, IWorkItem workItem, string documentPath)
        {
            //foreach (var automation in model.Automations)
            //{
            //    var response = await automation.Execute(model, workItem, documentPath, _serviceResolver);
            //    if (!response.Success)
            //    {
            //        return Response.FromFailure(response.Message);
            //    }
            //}

            //return Response.FromSuccess();
            throw new NotImplementedException();
        }
    }
}
