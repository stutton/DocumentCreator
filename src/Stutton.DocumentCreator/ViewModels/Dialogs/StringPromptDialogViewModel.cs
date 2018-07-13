using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.Dialogs
{
    public class StringPromptDialogViewModel : Observable
    {
        private string _promptMessage;

        public string PromptMessage
        {
            get => _promptMessage;
            set => Set(ref _promptMessage, value);
        }

        private string _inputString;

        public string InputString
        {
            get => _inputString;
            set => Set(ref _inputString, value);
        }
    }
}
