using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.Dialogs
{
    public abstract class MessageDialogViewModel : Observable
    {
        private string _message;

        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        internal MessageDialogViewModel(string message)
        {
            _message = message;
        }
    }
}
