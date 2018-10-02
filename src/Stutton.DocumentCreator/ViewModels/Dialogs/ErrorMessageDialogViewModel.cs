using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Stutton.DocumentCreator.Shared;

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

        #region CopySessionId Command

        private ICommand _copySessionIdCommand;
        public ICommand CopySessionIdCommand => _copySessionIdCommand ?? (_copySessionIdCommand = new RelayCommand(CopySessionId));

        private void CopySessionId()
        {
            Clipboard.SetText(SessionId.ToString());
        }

        #endregion
    }
}
