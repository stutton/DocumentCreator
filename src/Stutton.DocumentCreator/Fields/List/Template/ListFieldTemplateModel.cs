using System;
using System.Runtime.Serialization;
using System.Windows.Input;
using Stutton.DocumentCreator.Fields.List.Document;
using Stutton.DocumentCreator.Services.Image;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.List.Template
{
    public class ListFieldTemplateModel : FieldTemplateModelBase
    {
        private readonly IImageService _imageService;
        public const string Key = "ListField";

        public override Type DtoType => typeof(ListFieldTemplateDto);
        public override string Description => "A list of text and images created during document creation";
        public override string TypeDisplayName => "List";
        public override string FieldKey => Key;
        
        private string _name;
        public override string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public ListFieldTemplateModel(IImageService imageService)
        {
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        }

        public override FieldDocumentModelBase GetDocumentField()
        {
            var documentField = new ListFieldDocumentModel(_imageService)
            {
                Name = Name,
            };
            return documentField;
        }
    }
}