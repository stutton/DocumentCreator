using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignExtensions.Model;
using Stutton.DocumentCreator.Models.Documents.Details;

namespace Stutton.DocumentCreator.ViewModels.Documents.DocumentTemplateSteps
{
    public class DetailsStepViewModel
    {
        public DocumentDetailsModel Model { get; }

        public DetailsStepViewModel(DocumentDetailsModel model)
        {
            Model = model;
        }
    }
}
