using Stutton.DocumentCreator.Models.Documents;

namespace Stutton.DocumentCreator.ViewModels.Templates.TemplateSteps
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
