using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.ViewModels.Dialogs
{
    public class ErrorMessageDialogViewModel : MessageDialogViewModel
    {
        public ErrorMessageDialogViewModel(string message) : base(message)
        {
        }

        public ErrorMessageDialogViewModel(string message, Guid sessionId) : base(message)
        {
            SessionId = sessionId;
        }

        private Guid _sessionId;

        public Guid SessionId
        {
            get => _sessionId;
            set => Set(ref _sessionId, value);
        }
    }
}
