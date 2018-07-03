using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Fields.Text.Document;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.Text.Template
{
    public class TextFieldTemplateModel : FieldTemplateBase
    {
        public const string Key = "TextField";

        public override Type DtoType => typeof(TextFieldTemplateDto);
        public override string Description => $"Prompt to replace '{TextToReplace}'";
        public override string TypeDisplayName => "Text";
        public override string FieldKey => Key;

        private string _name;
        public override string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _textToReplace;
        public string TextToReplace
        {
            get => _textToReplace;
            set
            {
                if (Set(ref _textToReplace, value))
                {
                    RaisePropertyChanged(nameof(Description));
                }
            }
        }

        public override IFieldDocument GetDocumentField()
        {
            var documentField = new TextFieldDocumentModel()
            {
                Name = Name,
                TextToReplace = TextToReplace
            };
            return documentField;
        }
    }
}