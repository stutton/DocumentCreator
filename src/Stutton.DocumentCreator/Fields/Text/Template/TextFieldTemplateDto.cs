using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Fields.Text.Template
{
    [DataContract(Name = "TextField")]
    public class TextFieldTemplateDto : FieldTemplateDtoBase
    {
        [IgnoreDataMember]
        public override Type ModelType => typeof(TextFieldTemplateModel);
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string TextToReplace { get; set; }
    }
}
