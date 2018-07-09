using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Automations;
using Stutton.DocumentCreator.Fields;
using Stutton.DocumentCreator.Models.Template.Details;

namespace Stutton.DocumentCreator.Models.Template
{
    public class DocumentTemplateDto
    {
        public string Id { get; set; }
        public DocumentTemplateDetailsModel TemplateDetails { get; set; }
        public ICollection<FieldTemplateDtoBase> Fields { get; set; }
        public ICollection<IAutomation> Automations { get; set; }
    }
}
