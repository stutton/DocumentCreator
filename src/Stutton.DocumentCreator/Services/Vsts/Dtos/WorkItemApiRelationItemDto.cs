using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Services.Vsts.Dtos
{
    public sealed class WorkItemApiRelationItemDto
    {
        public string Url { get; set; }
        public WorkItemApiRelationAttributesDto Attributes { get; set; }
    }
}
