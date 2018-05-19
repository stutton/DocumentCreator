using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Documents;

namespace Stutton.DocumentCreator.ViewModels.Documents.DocumentTemplateSteps
{
    public class SummaryStepViewModel
    {
        public DocumentModel Model { get; }

        public SummaryStepViewModel(DocumentModel model)
        {
            Model = model;
        }
    }
}
