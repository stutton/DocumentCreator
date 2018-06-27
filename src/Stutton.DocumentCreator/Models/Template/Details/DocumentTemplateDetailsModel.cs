using System.Runtime.Serialization;
using Stutton.DocumentCreator.Models.Document.Details;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.Template.Details
{
    [DataContract(Name = "DocumentTemplateDetails")]
    public class DocumentTemplateDetailsModel : Observable
    {
        private string _description;
        private DocumentType _documentType;
        private string _name;
        private string _templateFilePath;
        private WorkItemQueryModel _workItemQuery = new WorkItemQueryModel();

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

        public DocumentDetailsModel GetDocumentDetailsModel()
        {
            var documentDetails = new DocumentDetailsModel
            {
                Name = Name,
                Description = Description,
                TemplateFilePath = TemplateFilePath,
                WorkItemQuery = WorkItemQuery,
                DocumentType = DocumentType
            };
            return documentDetails;
        }
    }
}