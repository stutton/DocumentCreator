using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Automations;
using Stutton.DocumentCreator.Fields;

namespace Stutton.DocumentCreator.Models.Document
{
    [DataContract(Name = "Document")]
    public class DocumentDto
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public int? SelectedWorkItemId { get; set; }

        [DataMember]
        public DocumentDetailsModel Details { get; set; }

        [DataMember]
        public List<FieldDocumentDtoBase> Fields { get; set; } = new List<FieldDocumentDtoBase>();

        [DataMember]
        public List<AutomationDtoBase> Automations { get; set; } = new List<AutomationDtoBase>();
    }
}
