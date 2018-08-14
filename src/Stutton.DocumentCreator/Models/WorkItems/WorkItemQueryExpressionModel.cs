﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Stutton.DocumentCreator.Shared;
using Unity.Interception.Utilities;

namespace Stutton.DocumentCreator.Models.WorkItems
{
    [DataContract(Name = "QueryExpression")]
    public class WorkItemQueryExpressionModel : Observable
    {
        private string _field;
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
