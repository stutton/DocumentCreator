using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Documents.Automations;

namespace Stutton.DocumentCreator.ViewModels.Documents.DocumentTemplateSteps
{
    public class AutomationsStepViewModel
    {
        public ObservableCollection<IAutomation> Automations { get; }

        public AutomationsStepViewModel(ObservableCollection<IAutomation> automations)
        {
            Automations = automations;
        }
    }
}
