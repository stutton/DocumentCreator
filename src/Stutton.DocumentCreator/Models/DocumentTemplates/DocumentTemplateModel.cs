using System.Runtime.Serialization;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.DocumentTemplates
{
    [DataContract(Name = "DocumentTemplate")]
    public class DocumentTemplateModel : Observable
    {
        private string _name;

        [DataMember]
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _templateFilePath;

        [DataMember]
        public string TemplateFilePath
        {
            get => _templateFilePath;
            set => Set(ref _templateFilePath, value);
        }

        private WorkItemQueryModel _workItemQuery;

        [DataMember]
        public WorkItemQueryModel WorkItemQuery
        {
            get => _workItemQuery;
            set => Set(ref _workItemQuery, value);
        }

        private DocumentType _documentType;

        [DataMember]
        public DocumentType DocumentType
        {
            get => _documentType;
            set => Set(ref _documentType, value);
        }
    }
}
