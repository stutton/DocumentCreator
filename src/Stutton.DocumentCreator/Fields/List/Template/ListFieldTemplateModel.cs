using System;
using System.Runtime.Serialization;
using System.Windows.Input;
using Stutton.DocumentCreator.Fields.List.Document;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.List.Template
{
    [DataContract(Name = "ListField")]
    public class ListFieldTemplateModel : FieldTemplateBase
    {
        public const string Key = "ListField";

        [IgnoreDataMember]
        public override string Description => "A list of text and images created during document creation";
        [IgnoreDataMember]
        public override string TypeDisplayName => "List";
        [IgnoreDataMember]
        public override string FieldKey => Key;
        
        private string _name;
        [DataMember]
        public override string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public override IFieldDocument GetDocumentField()
        {
            var documentField = new ListFieldDocumentModel
            {
                Name = Name,
            };
            return documentField;
        }
    }
}