using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.WorkItems;

namespace Stutton.DocumentCreator.Automations.SetWorkItemField
{
    [DataContract(Name = "SetWorkItemField")]
    public class SetWorkItemFieldAutomationDto : AutomationDtoBase
    {
        [DataMember]
        public override string Name { get; set; }
        [DataMember]
        public string SelectedField { get; set; }
        [DataMember]
        public string NewFieldValue { get; set; }
    }
}
