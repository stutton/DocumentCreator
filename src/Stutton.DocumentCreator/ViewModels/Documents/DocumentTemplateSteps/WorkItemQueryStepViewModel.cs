using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.WorkItems;

namespace Stutton.DocumentCreator.ViewModels.Documents.DocumentTemplateSteps
{
    public class WorkItemQueryStepViewModel
    {
        public WorkItemQueryModel Model { get; }

        public WorkItemQueryStepViewModel(WorkItemQueryModel model)
        {
            Model = model;
        }
    }
}
