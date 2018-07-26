using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.WorkItems
{
    [DataContract(Name = "WorkItemField")]
    public class WorkItemFieldModel : Observable
    {
        private string _name;

        [DataMember]
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _referenceName;

        [DataMember]
        public string ReferenceName
        {
            get => _referenceName;
            set => Set(ref _referenceName, value);
        }
    }
}
