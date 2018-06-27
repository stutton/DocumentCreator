using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.Document.Details
{
    [DataContract(Name = "DocumentDetails")]
    public class DocumentDetailsModel : Observable
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
    }
}
