using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Automations;
using Stutton.DocumentCreator.Fields;
using Stutton.DocumentCreator.Models.Document.Details;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.Document
{
    [DataContract(Name = "Document")]
    public class DocumentModel : Observable
    {
        [DataMember]
        public DocumentDetailsModel Details { get; }

        [DataMember]
        public ObservableCollection<IFieldDocument> Fields { get; }

        [DataMember]
        public ObservableCollection<IAutomation> Automations { get; }

        public DocumentModel(DocumentDetailsModel details, ObservableCollection<IFieldDocument> fields, ObservableCollection<IAutomation> automations)
        {
            Details = details ?? throw new ArgumentNullException(nameof(details));
            Fields = fields ?? throw new ArgumentNullException(nameof(fields));
            Automations = automations ?? throw new ArgumentNullException(nameof(automations));
        }
    }
}
