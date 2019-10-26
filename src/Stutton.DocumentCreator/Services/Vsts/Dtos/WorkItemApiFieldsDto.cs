using Newtonsoft.Json;

namespace Stutton.DocumentCreator.Services.Vsts.Dtos
{
    public class WorkItemApiFieldsDto
    {
        [JsonProperty("System.AreaPath")]
        public string Area { get; set; }

        [JsonProperty("System.TeamProject")]
        public string Team { get; set; }

        [JsonProperty("System.WorkItemType")]
        public string Type { get; set; }

        [JsonProperty("System.State")]
        public string State { get; set; }

        [JsonProperty("System.Title")]
        public string Title { get; set; }

        [JsonProperty("System.Description")]
        public string Description { get; set; }

        [JsonProperty("System.AssignedTo")]
        public WorkItemApiAssignedToDto AssignedTo { get; set; }
    }
}
