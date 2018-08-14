using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Automations;
using Stutton.DocumentCreator.Fields;
using Stutton.DocumentCreator.Models.Document;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.Template
{
    [DataContract(Name = "DocumentTemplate")]
    public class DocumentTemplateModel : Observable, IRequiresInitialization
    {
        [DataMember]
        public string Id { get; set; } = $@"{Guid.NewGuid()}";

        [DataMember]
        public DocumentTemplateDetailsModel TemplateDetails { get; set; } = new DocumentTemplateDetailsModel();

        [DataMember]
        public ObservableCollection<FieldTemplateModelBase> Fields { get; set; } = new ObservableCollection<FieldTemplateModelBase>();

        [DataMember]
        public ObservableCollection<AutomationModelBase> Automations { get; set; } = new ObservableCollection<AutomationModelBase>();

        public DocumentModel GetNewDocument()
        {
            var details = TemplateDetails.GetDocumentDetailsModel();
            var documentFields = GetDocumentFields();
            var documentAutomations = GetDocumentAutomations();
            return new DocumentModel(details, documentFields, documentAutomations);
        }

        private ObservableCollection<FieldDocumentModelBase> GetDocumentFields()
        {
            var documentFields = new ObservableCollection<FieldDocumentModelBase>();
            foreach (var fieldTemplate in Fields)
            {
                documentFields.Add(fieldTemplate.GetDocumentField());
            }

            return documentFields;
        }

        private ObservableCollection<AutomationModelBase> GetDocumentAutomations()
        {
            return new ObservableCollection<AutomationModelBase>(Automations);
        }

        public async Task<IResponse> Initialize()
        {
            foreach (var field in Fields)
            {
                if (field is IRequiresInitialization initializeMe)
                {
                    await initializeMe.Initialize();
                }
            }

            foreach (var automation in Automations)
            {
                if (automation is IRequiresInitialization initializeMe)
                {
                    await initializeMe.Initialize();
                }
            }

            return Response.FromSuccess();
        }
    }
}