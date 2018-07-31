using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Stutton.DocumentCreator.Models.Document;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Automations
{
    public abstract class AutomationModelBase : Observable
    {
        public event EventHandler<EventArgs> RequestDeleteMe;

        public abstract string TypeDisplayName { get; }

        public abstract string Name { get; set; }

        public abstract string Description { get; }

        #region Delete Command

        private ICommand _deleteCommand;
        [IgnoreDataMember]
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand(Delete));

        private void Delete()
        {
            RequestDeleteMe?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public abstract Task<IResponse> Execute(DocumentModel document, IWorkItem workItem, string documentPath);
    }
}
