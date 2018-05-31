using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.Documents.Steps
{
    public class SummaryStepViewModel : Observable
    {
        private DocumentModel _document;

        public DocumentModel Document
        {
            get => _document;
            set => Set(ref _document, value);
        }

        public SummaryStepViewModel(DocumentModel document)
        {
            _document = document;
        }
    }
}
