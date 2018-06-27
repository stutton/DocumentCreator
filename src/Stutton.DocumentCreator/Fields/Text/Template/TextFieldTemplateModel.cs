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
    [DataContract(Name = "TextField")]
    public class TextFieldTemplateModel : FieldTemplateBase
    {
        public const string Key = "TextField";
        
        [IgnoreDataMember]
        public override string Description => $"Prompt to replace '{TextToReplace}'";
        [IgnoreDataMember]
        public override string TypeDisplayName => "Text";
        [IgnoreDataMember]
        public override string FieldKey => Key;

        private string _name;
        [DataMember]
        public override string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _textToReplace;
        [DataMember]
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