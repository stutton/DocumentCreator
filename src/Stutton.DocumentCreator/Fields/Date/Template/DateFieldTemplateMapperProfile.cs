using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Stutton.DocumentCreator.Fields.Date.Template
{
    public class DateFieldTemplateMapperProfile : Profile
    {
        public DateFieldTemplateMapperProfile()
        {
            CreateMap<DateFieldTemplateModel, DateFieldTemplateDto>()
                .IncludeBase<FieldTemplateModelBase, FieldTemplateDtoBase>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}
