using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Document;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Automations.SetWorkItemField
{
    public class SetWorkItemFieldAutomationModel : AutomationModelBase
    {
        private readonly ITfsService _tfsService;

        public SetWorkItemFieldAutomationModel(ITfsService tfsService)
        {
            _tfsService = tfsService ?? throw new ArgumentNullException(nameof(tfsService));
        }

        public override string TypeDisplayName => "Set work item field";

        private string _name;
        public override string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public override string Description => "Set the value of a work item field to the given value";

        public override Task<IResponse> Execute(DocumentModel document, IWorkItem workItem, string documentPath)
        {
            throw new NotImplementedException();
        }
    }
}
