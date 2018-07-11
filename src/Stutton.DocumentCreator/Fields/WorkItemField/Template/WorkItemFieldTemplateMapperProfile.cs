using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Stutton.DocumentCreator.Fields.WorkItemField.Template
{
    public class WorkItemFieldTemplateMapperProfile : Profile
    {
        public WorkItemFieldTemplateMapperProfile()
        {
            CreateMap<WorkItemFieldTemplateModel, WorkItemFieldTemplateDto>()
                .IncludeBase<FieldTemplateModelBase, FieldTemplateDtoBase>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}
