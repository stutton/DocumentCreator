using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Windows.Input;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.WorkItems
{
    [DataContract(Name = "QueryExpression")]
    public class WorkItemQueryExpressionModel : Observable, IExpandable
    {
        public event EventHandler<EventArgs> RequestDeleteMe;

        private string _field;
        private bool _isExpanded;
        private WorkItemQueryExpressionOperator _operator;
        private string _value;
        private ObservableCollection<WorkItemQueryInValue> _values = new ObservableCollection<WorkItemQueryInValue>();

        public WorkItemQueryExpressionModel()
        {
            _values.CollectionChanged += ValuesOnCollectionChanged;
        }
        
        [DataMember]
        public string Field
        {
            get => _field;
            set => Set(ref _field, value);
        }

        [IgnoreDataMember]
        public bool IsExpanded
        {
            get => _isExpanded;
            set => Set(ref _isExpanded, value);
        }

        [DataMember]
        public WorkItemQueryExpressionOperator Operator
        {
            get => _operator;
            set => Set(ref _operator, value);
        }

        [DataMember]
        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        [DataMember]
        public ObservableCollection<WorkItemQueryInValue> Values
        {
            get => _values;
            set
            {
                if (!_values.Equals(value))
                {
                    _values.CollectionChanged -= ValuesOnCollectionChanged;
                    foreach (var inValue in _values)
                    {
                        inValue.RequestDeleteMe -= InValueOnRequestDeleteMe;
                    }
                }
                if (Set(ref _values, value))
                {
                    foreach (var inValue in _values)
                    {
                        inValue.RequestDeleteMe += InValueOnRequestDeleteMe;
                    }
                    _values.CollectionChanged += ValuesOnCollectionChanged;
                }
            }
        }

        #region Delete Command

        private ICommand _deleteCommand;
        [IgnoreDataMember]
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand(DeleteAsync));

        private void DeleteAsync()
        {
            RequestDeleteMe?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region AddInValue Command

        private ICommand _addInValueCommand;
        [IgnoreDataMember]
        public ICommand AddInValueCommand => _addInValueCommand ?? (_addInValueCommand = new RelayCommand(AddInValue));

        private void AddInValue()
        {
            var newInValue = new WorkItemQueryInValue();
            newInValue.RequestDeleteMe += InValueOnRequestDeleteMe;
            _values.Add(newInValue);
        }

        #endregion

        private void ValuesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var oldInValue = (WorkItemQueryInValue) oldItem;
                    oldInValue.RequestDeleteMe -= InValueOnRequestDeleteMe;
                }
            }

            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (var newItem in e.NewItems)
                {
                    var newInValue = (WorkItemQueryInValue) newItem;
                    newInValue.RequestDeleteMe += InValueOnRequestDeleteMe;
                }
            }

            RaisePropertyChanged(nameof(Values));
        }

        private void InValueOnRequestDeleteMe(object sender, EventArgs e)
        {
            var inValue = (WorkItemQueryInValue) sender;
            _values.Remove(inValue);
        }
    }
}
