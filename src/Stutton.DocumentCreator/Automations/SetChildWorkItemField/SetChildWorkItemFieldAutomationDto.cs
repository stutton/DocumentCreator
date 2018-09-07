using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Automations.SetChildWorkItemField
{
    [DataContract(Name = "SetChildWorkItemField")]
    public class SetChildWorkItemFieldAutomationDto : AutomationDtoBase
    {
        [DataMember]
        public override string Name { get; set; }

        [DataMember]
        public string SelectedField { get; set; }

        [DataMember]
        public string NewFieldValue { get; set; }
    }
}
