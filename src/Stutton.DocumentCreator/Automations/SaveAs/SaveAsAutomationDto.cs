using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Automations.SaveAs
{
    [DataContract(Name = "SaveAs")]
    public class SaveAsAutomationDto : AutomationDtoBase
    {
        [DataMember]
        public override string Name { get; set; }
        [DataMember]
        public string SavePath { get; set; }
    }
}
