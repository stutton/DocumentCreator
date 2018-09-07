using System;
using System.Runtime.Serialization;
using System.Windows.Input;
using Stutton.DocumentCreator.Fields.UserName.Document;
using Stutton.DocumentCreator.Services.Vsts;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.UserName.Template
{
    [DataContract(Name = "UserNameField")]
    public class UserNameFieldTemplateModel : FieldTemplateModelBase
    {
        private readonly IVstsService _vstsService;
        public const string Key = "UserNameField";

        public override Type DtoType => typeof(UserNameFieldTemplateDto);
        public override string Description => $"Replace '{TextToReplace}' with the current user's name";
        public override string TypeDisplayName => "Name";
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

        public UserNameFieldTemplateModel(IVstsService vstsService)
        {
            _vstsService = vstsService;
        }

        public override FieldDocumentModelBase GetDocumentField()
        {
            var documentField = new UserNameDocumentModel(_vstsService)
            {
                Name = Name,
                TextToReplace = TextToReplace
            };
            return documentField;
        }
    }
}
