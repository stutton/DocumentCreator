using System.Collections.Generic;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Stutton.DocumentCreator.Models.WorkItems;

namespace Stutton.DocumentCreator.Services.Vsts
{
    static class VstsWorkItemMapper
    {
        private static Dictionary<string, string> VstsNameToModelProperty = new Dictionary<string, string>
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
            foreach (var kv in VstsNameToModelProperty)
            {
                if (workItem.Fields.ContainsKey(kv.Key))
                {
                    var value = workItem.Fields[kv.Key].ToString();
                    typeof(WorkItemModel).GetProperty(kv.Value).SetValue(model, value);
                }
            }

            return model;
        }
    }
}
