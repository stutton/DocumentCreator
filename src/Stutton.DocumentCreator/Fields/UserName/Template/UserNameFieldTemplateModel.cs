using System;
using System.Runtime.Serialization;
using System.Windows.Input;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.UserName.Template
{
    [DataContract(Name = "UserNameField")]
    public class UserNameFieldTemplateModel : Observable, IFieldTemplate
    {
        public const string Key = "UserNameField";

        public string Description => $"Replace '{TextToReplace}' with the current user's name";
        public string TypeDisplayName => "Name";
        public string FieldKey => Key;

        private string _name;
        [DataMember]
        public string Name
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

        public event EventHandler<IFieldTemplate> RequestDeleteMe;

        #region Delete Command

        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand(Delete));

        private void Delete()
        {
            RequestDeleteMe?.Invoke(this, this);
        }

        #endregion
    }
}
