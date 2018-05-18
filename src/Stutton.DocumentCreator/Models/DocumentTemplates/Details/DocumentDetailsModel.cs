using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.DocumentTemplates.Details
{
    [DataContract(Name = "DocumentDetails")]
    public class DocumentDetailsModel : Observable
    {
        private string _name;

        [DataMember]
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _description;

        [DataMember]
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
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
