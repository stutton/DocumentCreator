using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.WorkItems;

namespace Stutton.DocumentCreator.Fields.WorkItemField.Template
{
    [DataContract(Name = "WorkItemField")]
    public class WorkItemFieldTemplateDto : FieldTemplateDtoBase
    {
        [DataMember]
        public override string Name { get; set; }
        [DataMember]
        public string TextToReplace { get; set; }
        [DataMember]
        public WorkItemFieldModel SelectedField { get; set; }
    }
}
