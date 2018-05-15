using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.WorkItems
{
    [DataContract(Name = "InValue")]
    public class WorkItemQueryInValue : Observable
    {
        private string _value;

        [DataMember]
        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }
    }
}
