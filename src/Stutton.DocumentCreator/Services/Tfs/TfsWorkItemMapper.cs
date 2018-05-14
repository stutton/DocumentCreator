using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Stutton.DocumentCreator.Models.WorkItems;

namespace Stutton.DocumentCreator.Services.Tfs
{
    static class TfsWorkItemMapper
    {
        private static Dictionary<string, string> TfsNameToModelProperty = new Dictionary<string, string>
        {
            {"System.WorkItemType", nameof(WorkItemModel.Type)},
            {"System.AssignedTo", nameof(WorkItemModel.AssignedTo)},
            {"System.Title", nameof(WorkItemModel.Title)},
            {"System.Description", nameof(WorkItemModel.Description)},
            {"System.State", nameof(WorkItemModel.State)},
            {"System.AreaPath", nameof(WorkItemModel.Area)}
        };

        public static WorkItemModel MapToModel(WorkItem workItem)
        {
            var model = new WorkItemModel();
            model.Id = workItem.Id.Value;
            foreach (var kv in TfsNameToModelProperty)
            {
                var value = workItem.Fields[kv.Key].ToString();
                typeof(WorkItemModel).GetProperty(kv.Value).SetValue(model, value);
            }

            return model;
        }
    }
}
