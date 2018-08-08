using System;
using Stutton.DocumentCreator.Fields.Date.Document;

namespace Stutton.DocumentCreator.Fields.Date.Template
{
    public class DateFieldTemplateModel : FieldTemplateModelBase
    {
        public const string Key = "DateField";

        public override Type DtoType => typeof(DateFieldTemplateDto);
        public override string Description => $"Replace '{TextToReplace}' with the current date";
        public override string TypeDisplayName => "Date";
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

        public override FieldDocumentModelBase GetDocumentField()
        {
            var documentField = new DateFieldDocumentModel()
            {
                Name = Name,
                TextToReplace = TextToReplace
            };
            return documentField;
        }
    }
}
