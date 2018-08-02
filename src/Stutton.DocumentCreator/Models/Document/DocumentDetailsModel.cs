using System.Runtime.Serialization;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.Document
{
    [DataContract(Name = "DocumentDetails")]
    public class DocumentDetailsModel : Observable
    {
        private string _description;
        private DocumentType _documentType;
        private string _name;
        private string _templateFilePath;
        private WorkItemQueryModel _workItemQuery = new WorkItemQueryModel();
        private string _generatedFileName;

        [DataMember]
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        [DataMember]
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        [DataMember]
        public string TemplateFilePath
        {
            get => _templateFilePath;
            set => Set(ref _templateFilePath, value);
        }

        [DataMember]
        public string GeneratedFileName
        {
            get => _generatedFileName;
            set => Set(ref _generatedFileName, value);
        }

        [DataMember]
        public WorkItemQueryModel WorkItemQuery
        {
            get => _workItemQuery;
            set => Set(ref _workItemQuery, value);
        }

        [DataMember]
        public DocumentType DocumentType
        {
            get => _documentType;
            set => Set(ref _documentType, value);
        }
    }
}
