using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.Toolbar
{
    public class SearchBarOptions : Observable
    {
        private bool _isShown;

        public bool IsShown
        {
            get => _isShown;
            set => Set(ref _isShown, value);
        }

        private string _hint;

        public string Hint
        {
            get => _hint;
            set => Set(ref _hint, value);
        }

        private string _text;

        public string Text
        {
            get => _text;
            set => Set(ref _text, value);
        }
    }
}
