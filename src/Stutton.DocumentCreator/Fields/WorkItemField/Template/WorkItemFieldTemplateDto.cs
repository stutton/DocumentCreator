using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Fields.WorkItemField.Template
{
    [DataContract(Name = "WorkItemField")]
    public class WorkItemFieldTemplateDto : FieldTemplateDtoBase
    {
        [IgnoreDataMember]
        public override Type ModelType => typeof(WorkItemFieldTemplateModel);
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string TextToReplace { get; set; }
        [DataMember]
        public string SelectedField { get; set; }
    }
}
