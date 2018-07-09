using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Stutton.DocumentCreator.Automations;
using Stutton.DocumentCreator.Fields;
using Stutton.DocumentCreator.Models.Document;
using Stutton.DocumentCreator.Models.Template.Details;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.Template
{
    [DataContract(Name = "DocumentTemplate")]
    public class DocumentTemplateModel : Observable
    {
        [DataMember]
        public string Id { get; set; } = $@"{Guid.NewGuid()}";

        [DataMember]
        public DocumentTemplateDetailsModel TemplateDetails { get; set; } = new DocumentTemplateDetailsModel();

        [DataMember]
        public ObservableCollection<FieldTemplateBase> Fields { get; set; } = new ObservableCollection<FieldTemplateBase>();

        [DataMember]
        public ObservableCollection<IAutomation> Automations { get; set; } = new ObservableCollection<IAutomation>();

        public DocumentModel GetNewDocument()
        {
            var details = TemplateDetails.GetDocumentDetailsModel();
            var documentFields = GetDocumentFields();
            var documentAutomations = GetDocumentAutomations();
            return new DocumentModel(details, documentFields, documentAutomations);
        }

        private ObservableCollection<IFieldDocument> GetDocumentFields()
        {
            var documentFields = new ObservableCollection<IFieldDocument>();
            foreach (var fieldTemplate in Fields)
            {
                documentFields.Add(fieldTemplate.GetDocumentField());
            }

            return documentFields;
        }

        private ObservableCollection<IAutomation> GetDocumentAutomations()
        {
            return new ObservableCollection<IAutomation>(Automations);
        }
    }
}