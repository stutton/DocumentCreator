using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Stutton.DocumentCreator.Models.DocumentTemplates.Automations;
using Stutton.DocumentCreator.Models.DocumentTemplates.Details;
using Stutton.DocumentCreator.Models.DocumentTemplates.Fields;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.DocumentTemplates
{
    [DataContract(Name = "DocumentTemplate")]
    public class DocumentTemplateModel : Observable
    {
        [DataMember]
        public DocumentDetailsModel Details { get; } = new DocumentDetailsModel();

        [DataMember]
        public ObservableCollection<IField> Fields { get; } = new ObservableCollection<IField>();

        [DataMember]
        public ObservableCollection<IAutomation> Automations { get; } = new ObservableCollection<IAutomation>();
    }
}
