using System;
using System.Runtime.Serialization;
using System.Windows.Input;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields
{
    public abstract class FieldTemplateModelBase : Observable, IExpandable
    {
        public event EventHandler<FieldTemplateModelBase> RequestDeleteMe;

        public abstract Type DtoType { get; }
        public abstract string Description { get; }
        public abstract string TypeDisplayName { get; }
        public abstract string FieldKey { get; }
        public abstract string Name { get; set; }

        private bool _isExpanded;
        [IgnoreDataMember]
        public bool IsExpanded
        {
            get => _isExpanded;
            set => Set(ref _isExpanded, value);
        }

        #region Delete Command

        private ICommand _deleteCommand;
        [IgnoreDataMember]
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand(Delete));

        private void Delete()
        {
            RequestDeleteMe?.Invoke(this, this);
        }

        #endregion

        public abstract FieldDocumentModelBase GetDocumentField();
    }
}