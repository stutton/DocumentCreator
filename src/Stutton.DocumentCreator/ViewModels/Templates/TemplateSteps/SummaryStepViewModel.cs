using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.Template;

namespace Stutton.DocumentCreator.ViewModels.Templates.TemplateSteps
{
    public class SummaryStepViewModel
    {
        public DocumentTemplateModel Model { get; }

        public SummaryStepViewModel(DocumentTemplateModel model)
        {
            Model = model;
        }
    }
}
