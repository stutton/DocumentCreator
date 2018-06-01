using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Document
{
    public class DocumentService : IDocumentService
    {
        private static readonly string TempDirectory = Path.GetTempPath();

        public async Task<IResponse<string>> CreateDocument(DocumentModel model)
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
                    var response = await field.GetReplaceWithText();
                    if (!response.Success)
                    {
                        // TODO: Should we fail if a field does?
                        return Response<string>.FromFailure(response.Message);
                    }

                    var replaceWith = response.Value;
                    await Task.Run(() => TextReplacer.SearchAndReplace(doc, field.TextToReplace, replaceWith, false));
                }
            }

            return Response<string>.FromSuccess(outFile);
        }
    }
}
