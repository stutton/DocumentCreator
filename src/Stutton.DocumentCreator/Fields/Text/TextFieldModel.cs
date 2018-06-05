using System;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.Text
{
    public class TextFieldModel : Observable, IField
    {
        public const string Key = "TextField";
        private string _name;

        private string _replaceWithText;
        private string _textToReplace;

        public string ReplaceWithText
        {
            get => _replaceWithText;
            set => Set(ref _replaceWithText, value);
        }

        public string Description => $"Replace '{TextToReplace}' with '{ReplaceWithText}'";

        public string TypeDisplayName => "Text";
        public string FieldKey => Key;

        public string TextToReplace
        {
            get => _textToReplace;
            set => Set(ref _textToReplace, value);
        }

        public async Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem, IServiceResolver serviceResolver)
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