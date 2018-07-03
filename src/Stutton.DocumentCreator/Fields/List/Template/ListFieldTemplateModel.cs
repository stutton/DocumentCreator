using System;
using System.Runtime.Serialization;
using System.Windows.Input;
using Stutton.DocumentCreator.Fields.List.Document;
using Stutton.DocumentCreator.Services.Image;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.List.Template
{
    [DataContract(Name = "ListField")]
    public class ListFieldTemplateModel : FieldTemplateBase
    {
        private readonly IImageService _imageService;
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

        public ListFieldTemplateModel(IImageService imageService)
        {
            _imageService = imageService;
        }

        public override IFieldDocument GetDocumentField()
        {
            var documentField = new ListFieldDocumentModel(_imageService)
            {
                Name = Name,
            };
            return documentField;
        }
    }
}