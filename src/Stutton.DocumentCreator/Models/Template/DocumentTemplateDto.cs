using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Automations;
using Stutton.DocumentCreator.Fields;

namespace Stutton.DocumentCreator.Models.Template
{
    public class DocumentTemplateDto
    {
        public string Id { get; set; }
        public DocumentTemplateDetailsModel TemplateDetails { get; set; }
        public ICollection<FieldTemplateDtoBase> Fields { get; set; }
        public ICollection<AutomationDtoBase> Automations { get; set; }
    }
}
