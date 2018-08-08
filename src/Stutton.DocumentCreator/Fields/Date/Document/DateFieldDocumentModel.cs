using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.Date.Document
{
    public class DateFieldDocumentModel : FieldDocumentModelBase
    {
        public const string Key = "DateField";

        public override string Description => $"Replace '{TextToReplace}' with the current date";
        public override string FieldKey => Key;

        private string _textToReplace;
        public string TextToReplace
        {
            get => _textToReplace;
            set => Set(ref _textToReplace, value);
        }

        public override async Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem)
        {
            try
            {
                await Task.Run(() => TextReplacer.SearchAndReplace(document, TextToReplace, DateTime.Now.ToShortDateString(), false));
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException("Failed to add date to document", ex);
            }
        }
    }
}
