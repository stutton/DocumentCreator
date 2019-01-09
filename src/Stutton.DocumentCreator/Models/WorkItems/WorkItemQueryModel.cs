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

        public WorkItemQueryModel()
        {
            Expressions.CollectionChanged += Expressions_CollectionChanged;
        }

        private void Expressions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            if(args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach(var item in args.NewItems)
                {
                    var model = (WorkItemQueryExpressionModel)item;
                    model.RequestDeleteMe += (s, e) => Expressions.Remove(model);
                }
            }
        }
    }
}
