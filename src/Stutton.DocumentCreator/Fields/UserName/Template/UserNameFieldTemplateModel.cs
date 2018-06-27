using System;
using System.Runtime.Serialization;
using System.Windows.Input;
using Stutton.DocumentCreator.Fields.UserName.Document;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.UserName.Template
{
    [DataContract(Name = "UserNameField")]
    public class UserNameFieldTemplateModel : FieldTemplateBase
    {
        [IgnoreDataMember]
        private readonly ITfsService _tfsService;
        public const string Key = "UserNameField";

        [IgnoreDataMember]
        public override string Description => $"Replace '{TextToReplace}' with the current user's name";
        [IgnoreDataMember]
        public override string TypeDisplayName => "Name";
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
            set => Set(ref _textToReplace, value);
        }

        public UserNameFieldTemplateModel(ITfsService tfsService)
        {
            _tfsService = tfsService;
        }

        public override IFieldDocument GetDocumentField()
        {
            var documentField = new UserNameDocumentModel(_tfsService)
            {
                Name = Name,
                TextToReplace = TextToReplace
            };
            return documentField;
        }
    }
}
