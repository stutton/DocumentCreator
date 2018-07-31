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
    public class DocumentModel : Observable
    {
        private string _fileName;
        
        public string FileName
        {
            get => _fileName;
            set => Set(ref _fileName, value);
        }

        private int? _selectedWorkItemId;

        public int? SelectedWorkItemId
        {
            get => _selectedWorkItemId;
            set => Set(ref _selectedWorkItemId, value);
        }
        
        public DocumentDetailsModel Details { get; }
        
        public ObservableCollection<FieldDocumentModelBase> Fields { get; }
        
        public ObservableCollection<AutomationModelBase> Automations { get; }

        public DocumentModel(DocumentDetailsModel details, ObservableCollection<FieldDocumentModelBase> fields, ObservableCollection<AutomationModelBase> automations)
        {
            Details = details ?? throw new ArgumentNullException(nameof(details));
            Fields = fields ?? throw new ArgumentNullException(nameof(fields));
            Automations = automations ?? throw new ArgumentNullException(nameof(automations));
        }
    }
}
