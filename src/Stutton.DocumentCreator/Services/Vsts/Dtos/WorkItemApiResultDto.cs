using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Services.Vsts.Dtos
{
    public sealed class WorkItemApiResultDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public WorkItemApiFieldsDto Fields { get; set; }
        public IEnumerable<WorkItemApiRelationItemDto> Relations { get; set; }
    }
}
