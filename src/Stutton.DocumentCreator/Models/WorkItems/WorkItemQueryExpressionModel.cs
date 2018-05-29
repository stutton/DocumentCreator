using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.WorkItems
{
    [DataContract(Name = "QueryExpression")]
    public class WorkItemQueryExpressionModel : Observable
    {
        private string _fieldName;
        private WorkItemQueryExpressionOperator _operator;
        private string _value;
        private ObservableCollection<WorkItemQueryInValue> _values = new ObservableCollection<WorkItemQueryInValue>();

        [DataMember]
        public string FieldName
        {
            get => _fieldName;
            set => Set(ref _fieldName, value);
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
            set => Set(ref _values, value);
        }
    }
}
