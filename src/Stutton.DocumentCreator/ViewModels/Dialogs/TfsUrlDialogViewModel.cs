using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.Dialogs
{
    public class TfsUrlDialogViewModel : Observable
    {
        private string _tfsUrl;

        public string TfsUrl
        {
            get => _tfsUrl;
            set => Set(ref _tfsUrl, value);
        }
    }
}
