using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.Text
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

        public string Description => $"Replace '{TextToReplace}' with '{ReplaceWithText}'";

        public string TypeDisplayName => "Text";
        public string FieldKey => Key;

        public string TextToReplace
        {
            get => _textToReplace;
            set => Set(ref _textToReplace, value);
        }

        public Task<IResponse<string>> GetReplaceWithText()
        {
            return Task.FromResult((IResponse<string>)Response<string>.FromSuccess(ReplaceWithText));
        }
    }
}