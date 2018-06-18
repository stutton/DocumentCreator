using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.Text.Document
{
    public class TextFieldDocumentModel : Observable, IFieldDocument
    {
        public event EventHandler<IFieldTemplate> RequestDeleteMe;
        private string _name;

        public string Name { get; }
        public string Description { get; }
        public string TypeDisplayName { get; }
        public string FieldKey { get; }
        public string TextToReplace { get; }

        private string _replaceWithText;

        public string ReplaceWithText
        {
            get => _replaceWithText;
            set => Set(ref _replaceWithText, value);
        }

        public async Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem)
        {
            try
            {
                await Task.Run(() => TextReplacer.SearchAndReplace(document, TextToReplace, ReplaceWithText, false));
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException("Error replacing text", ex);
            }
        }
    }
}
