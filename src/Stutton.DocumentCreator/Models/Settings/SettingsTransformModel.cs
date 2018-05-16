using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.WorkItems;

namespace Stutton.DocumentCreator.Models.Settings
{
    [DataContract(Name = "SettingsTransform")]
    public class SettingsTransformModel
    {
        [DataMember]
        public string TfsUrl { get; set; }
        [DataMember]
        public string TfsDefaultCollection { get; set; }
        [DataMember]
        public WorkItemQueryModel WorkItemQuery { get; set; }
    }
}
