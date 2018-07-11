using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Stutton.DocumentCreator.Fields.List.Template;
using Stutton.DocumentCreator.Fields.Text.Template;
using Stutton.DocumentCreator.Fields.UserName.Template;
using Stutton.DocumentCreator.Fields.WorkItemField.Template;
using Stutton.DocumentCreator.Models.Template;

namespace Stutton.DocumentCreator.Fields
{
    public class FieldTemplateMapperProfile : Profile
    {
        public FieldTemplateMapperProfile()
        {
            CreateMap<FieldTemplateModelBase, FieldTemplateDtoBase>().ReverseMap();
        }
    }
}
