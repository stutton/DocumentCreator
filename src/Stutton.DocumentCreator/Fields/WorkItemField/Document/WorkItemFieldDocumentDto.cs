using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.WorkItems;

namespace Stutton.DocumentCreator.Fields.WorkItemField.Document
{
    [DataContract(Name = "WorkItemField")]
    public sealed class WorkItemFieldDocumentDto : FieldDocumentDtoBase
    {
        [DataMember]
        public override string Name { get; set; }
        [DataMember]
        public string TextToReplace { get; set; }
        [DataMember]
        public string SelectedField { get; set; }
    }
}
