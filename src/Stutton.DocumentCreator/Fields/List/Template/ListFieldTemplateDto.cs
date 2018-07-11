using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Fields.List.Template
{
    [DataContract(Name = "ListField")]
    public class ListFieldTemplateDto : FieldTemplateDtoBase
    {
        [DataMember]
        public override string Name { get; set; }
    }
}
