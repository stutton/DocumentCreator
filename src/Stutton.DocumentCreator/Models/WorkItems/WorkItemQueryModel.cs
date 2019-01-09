using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Models.WorkItems
{
    [DataContract(Name = "WorkItemQuery")]
    public class WorkItemQueryModel
    {
        [DataMember]
        public ObservableCollection<WorkItemQueryExpressionModel> Expressions { get; } = new ObservableCollection<WorkItemQueryExpressionModel>();

        public void AddExpression(WorkItemQueryExpressionModel expression)
        {
            expression.RequestDeleteMe += (s, e) => Expressions.Remove(expression);
            Expressions.Add(expression);
        }
    }
}
