using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Stutton.DocumentCreator.Fields.List.Template
{
    public class ListFieldTemplateMapperProfile : Profile
    {
        public ListFieldTemplateMapperProfile()
        {
            CreateMap<ListFieldTemplateModel, ListFieldTemplateDto>()
                .IncludeBase<FieldTemplateModelBase, FieldTemplateDtoBase>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}
