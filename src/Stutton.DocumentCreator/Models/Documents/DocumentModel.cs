using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Stutton.DocumentCreator.Models.Documents.Automations;
using Stutton.DocumentCreator.Models.Documents.Details;
using Stutton.DocumentCreator.Models.Documents.Fields;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.Documents
{
    [DataContract(Name = "DocumentTemplate")]
    public class DocumentModel : Observable
    {
        [DataMember] public DocumentDetailsModel Details { get; } = new DocumentDetailsModel();

        [DataMember] public ObservableCollection<IField> Fields { get; } = new ObservableCollection<IField>();

        [DataMember]
        public ObservableCollection<IAutomation> Automations { get; } = new ObservableCollection<IAutomation>();
    }
}