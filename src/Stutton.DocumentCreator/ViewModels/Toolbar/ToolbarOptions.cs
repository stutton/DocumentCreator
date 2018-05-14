using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.Toolbar
{
    public class ToolbarOptions : Observable
    {
        private ButtonOptions _refresh = new ButtonOptions();

        public ButtonOptions Refresh
        {
            get => _refresh;
            set => Set(ref _refresh, value);
        }

        private ButtonOptions _save = new ButtonOptions();

        public ButtonOptions Save
        {
            get => _save;
            set => Set(ref _save, value);
        }

        private ButtonOptions _export = new ButtonOptions();

        public ButtonOptions Export
        {
            get => _export;
            set => Set(ref _export, value);
        }

        private SearchBarOptions _searchBar = new SearchBarOptions();

        public SearchBarOptions SearchBar
        {
            get => _searchBar;
            set => Set(ref _searchBar, value);
        }
    }
}
