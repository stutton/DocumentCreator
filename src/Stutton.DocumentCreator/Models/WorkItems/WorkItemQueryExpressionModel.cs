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
        private ObservableCollection<string> _values = new ObservableCollection<string>();

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
        public ObservableCollection<string> Values
        {
            get => _values;
            set => Set(ref _values, value);
        }
    }
}
