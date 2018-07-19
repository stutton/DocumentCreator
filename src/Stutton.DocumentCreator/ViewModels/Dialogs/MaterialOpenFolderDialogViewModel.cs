using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.Dialogs
{
    public class MaterialOpenFolderDialogViewModel : Observable
    {
        private string _selectedFolder;

        public string SelectedFolder
        {
            get => _selectedFolder;
            set => Set(ref _selectedFolder, value);
        }
    }
}
