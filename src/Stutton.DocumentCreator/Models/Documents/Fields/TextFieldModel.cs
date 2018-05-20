using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.Documents.Fields
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

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string TypeDisplayName => "Text";
        public string FieldKey => Key;

        public string TextToReplace
        {
            get => _textToReplace;
            set => Set(ref _textToReplace, value);
        }
    }
}