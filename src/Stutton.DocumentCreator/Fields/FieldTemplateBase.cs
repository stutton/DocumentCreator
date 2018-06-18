using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields
{
    public abstract class FieldTemplateBase : Observable, IFieldTemplate
    {
        public event EventHandler<IFieldTemplate> RequestDeleteMe;

        public abstract string Description { get; }
        public abstract string TypeDisplayName { get; }
        public abstract string FieldKey { get; }
        public abstract string Name { get; set; }

        #region Delete Command

        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand(Delete));

        private void Delete()
        {
            RequestDeleteMe?.Invoke(this, this);
        }

        #endregion
    }
}
