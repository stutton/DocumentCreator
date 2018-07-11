using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.Text.Document
{
    [DataContract(Name = "TextField")]
    public class TextFieldDocumentModel : FieldDocumentModelBase
    {
        public const string Key = "TextField";
        
        public override string Description => $"Prompt to replace '{TextToReplace}'";
        public string TypeDisplayName => "Text";
        public override string FieldKey => Key;

        private string _textToReplace;
        [DataMember]
        public string TextToReplace
        {
            get => _textToReplace;
            set => Set(ref _textToReplace, value);
        }

        private string _replaceWithText;
        [DataMember]
        public string ReplaceWithText
        {
            get => _replaceWithText;
            set => Set(ref _replaceWithText, value);
        }

        public override async Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem)
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
